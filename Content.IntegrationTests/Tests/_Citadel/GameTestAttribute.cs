// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System.Collections.Generic;
using System.Reflection;
using Content.IntegrationTests.Pair;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Content.IntegrationTests.Tests._Citadel;

/// <summary>
///     Marks a game test, that needs a client and server to run.
/// </summary>
/// <typeparam name="TData"></typeparam>
public sealed class GameTestAttribute<TData> : Attribute, ITestBuilder, IImplyFixture, IApplyToTest
    where TData: GameTestData, new()
{
    /// <summary>
    ///     An optional description of the test.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Evil magic that allows us to cleanly wrap a test method.
    ///     This is used instead of a 'simple' closure because of the need to preserve the object being invoked on.
    /// </summary>
    private sealed class TestDataBasedWrapper(IMethodInfo inner) : IMethodInfo
    {
        public T[] GetCustomAttributes<T>(bool inherit) where T : class
        {
            return Array.Empty<T>();
        }

        public bool IsDefined<T>(bool inherit) where T : class
        {
            return false;
        }

        public IParameterInfo[] GetParameters()
        {
            return Array.Empty<IParameterInfo>();
        }

        public Type[] GetGenericArguments()
        {
            return Array.Empty<Type>();
        }

        public IMethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            throw new NotSupportedException();
        }

        public object Invoke(object fixture, params object[] args)
        {
            return InnerInvoke(fixture);
        }

        private async Task InnerInvoke(object fixture)
        {
            // We don't use the fixture at all..
            var data = new TData();

            await data.DoSetup();

            try
            {
                if (inner.ReturnType.IsType(typeof(Task)))
                {
                    await (Task)inner.Invoke(fixture, data)!;
                }
                else
                {
                    inner.Invoke(fixture, data);
                }
            }
            catch (Exception)
            {
                data.MarkDirty();
                throw;
            }
            finally
            {
                await data.DoTeardown();
            }
        }

        public ITypeInfo TypeInfo => new TypeWrapper(((Func<Task>)(HackToLookAsync)).GetType());
        public MethodInfo MethodInfo => ((Func<Task>)(HackToLookAsync)).Method;
        public string Name => inner.Name;
        public bool IsAbstract => false;
        public bool IsPublic => true;
        public bool IsStatic => false;
        public bool ContainsGenericParameters => false;
        public bool IsGenericMethod => false;
        public bool IsGenericMethodDefinition => false;
        public ITypeInfo ReturnType => new TypeWrapper(typeof(void));

        private Task HackToLookAsync()
        {
            return Task.CompletedTask;
        }
    }

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
    {
        var innerParams = method.GetParameters();

        if (innerParams.Length == 1 && innerParams[0].ParameterType.IsAssignableTo(typeof(GameTestData)))
        {
            var wrapper = new TestDataBasedWrapper(method);

            return new[] { new TestMethod(wrapper, null) };
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public void ApplyToTest(Test test)
    {
        if (!test.Properties.ContainsKey(PropertyNames.Description) && Description is not null)
            test.Properties.Set(PropertyNames.Description, Description);
    }
}

public sealed class DirtyFlag
{
    public bool IsDirty { get; private set; }

    public void Set() => IsDirty = true;
}

/// <summary>
///     A simpler version of the generic GameTestAttribute that allows you to specify what you need with just arguments.
/// </summary>
public sealed class GameTestAttribute : Attribute, ITestBuilder, IImplyFixture, IApplyToTest
{
    /// <summary>
    ///     An optional description of the test.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Evil magic that allows us to cleanly wrap a test method.
    ///     This is used instead of a 'simple' closure because of the need to preserve the object being invoked on.
    /// </summary>
    private sealed class AttributeBasedWrapper(IMethodInfo inner) : IMethodInfo
    {
        public T[] GetCustomAttributes<T>(bool inherit) where T : class
        {
            return Array.Empty<T>();
        }

        public bool IsDefined<T>(bool inherit) where T : class
        {
            return false;
        }

        public IParameterInfo[] GetParameters()
        {
            return Array.Empty<IParameterInfo>();
        }

        public Type[] GetGenericArguments()
        {
            return Array.Empty<Type>();
        }

        public IMethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            throw new NotSupportedException();
        }

        public object Invoke(object fixture, params object[] args)
        {
            return InnerInvoke(fixture);
        }

        private async Task InnerInvoke(object fixture)
        {
            var pair = await PoolManager.GetServerClient(new PoolSettings { Connected = true });

            var args = new List<object>();

            var dirty = new DirtyFlag();

            foreach (var param in inner.GetParameters())
            {
                if (param.GetCustomAttributes<SidedDependencyAttribute>(false) is [var dependencyAttribute])
                {
                    if (dependencyAttribute.Side is Side.Server)
                    {
                        args.Add(pair.Server.InstanceDependencyCollection.ResolveType(param.ParameterType));
                    }
                    else
                    {
                        args.Add(pair.Client.InstanceDependencyCollection.ResolveType(param.ParameterType));
                    }
                }
                else if (param.GetCustomAttributes<SystemAttribute>(false) is [var systemAttribute])
                {
                    if (systemAttribute.Side is Side.Server)
                    {
                        args.Add(pair.Server.EntMan.EntitySysManager.GetEntitySystem(param.ParameterType));
                    }
                    else
                    {
                        args.Add(pair.Client.EntMan.EntitySysManager.GetEntitySystem(param.ParameterType));
                    }
                }
                else if (param.ParameterType == typeof(TestPair))
                {
                    args.Add(pair);
                }
                else if (param.ParameterType == typeof(DirtyFlag))
                {
                    args.Add(dirty);
                }
            }

            try
            {
                if (inner.ReturnType.IsType(typeof(Task)))
                {
                    await (Task)inner.Invoke(fixture, args.ToArray())!;
                }
                else
                {
                    inner.Invoke(fixture, args.ToArray());
                }
            }
            catch (Exception)
            {
                dirty.Set();
                throw;
            }
            finally
            {
                if (!dirty.IsDirty)
                    await pair.CleanReturnAsync();
                else
                    await pair.DisposeAsync();
            }
        }

        public ITypeInfo TypeInfo => new TypeWrapper(((Func<Task>)(HackToLookAsync)).GetType());
        public MethodInfo MethodInfo => ((Func<Task>)(HackToLookAsync)).Method;
        public string Name => inner.Name;
        public bool IsAbstract => false;
        public bool IsPublic => true;
        public bool IsStatic => false;
        public bool ContainsGenericParameters => false;
        public bool IsGenericMethod => false;
        public bool IsGenericMethodDefinition => false;
        public ITypeInfo ReturnType => new TypeWrapper(typeof(void));

        private Task HackToLookAsync()
        {
            return Task.CompletedTask;
        }
    }

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
    {
        var innerParams = method.GetParameters();

        if (innerParams.Length == 1 && innerParams[0].ParameterType.IsAssignableTo(typeof(GameTestData)))
        {
            throw new NotSupportedException();
        }
        else
        {
            var wrapper = new AttributeBasedWrapper(method);

            return new[] { new TestMethod(wrapper, null) };
        }
    }

    public void ApplyToTest(Test test)
    {
        if (!test.Properties.ContainsKey(PropertyNames.Description) && Description is not null)
            test.Properties.Set(PropertyNames.Description, Description);
    }
}
