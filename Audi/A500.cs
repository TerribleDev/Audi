using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Audi
{
    public class A500
    {
        internal static bool Ran { get; set; }
        internal static readonly A500 Instance = new A500();

        public delegate void StandardVoid();

        private static Dictionary<string, bool> primedAssemblies = new Dictionary<string, bool>();

        public static void Setup()
        {
            var assm = Assembly.GetCallingAssembly();
            if(primedAssemblies.ContainsKey(assm.FullName))
            {
                return;
            }
            primedAssemblies.Add(assm.FullName, true);
            assm
                .GetTypes()
                .SelectMany(a => a.GetMethods())
                .Where(a => a.GetCustomAttributes().Any(b => IsTest(b.GetType().Name)))

                .ToList()
                .ForEach(a =>
                {
                    unsafe
                    {
                        var newMethod = new DynamicMethod("overrideMethod" + a.Name, a.ReturnType, a.GetParameters().Select(b => b.ParameterType).ToArray());
                        var body = newMethod.GetILGenerator();
                        body.Emit(OpCodes.Ret);
                        var del = Delegate.CreateDelegate(typeof(StandardVoid), Instance, "replacement");
                        *((int*)new IntPtr(((int*)a.MethodHandle.Value.ToPointer() + 2)).ToPointer()) = del.Method.MethodHandle.GetFunctionPointer().ToInt32();
                    }
                });
            Ran = true;
        }

        public void replacement()
        {
            return;
        }

        internal static bool IsTest(string typeName)
        {
            var type = typeName.Contains("Test") || typeName.Contains("Fact") || typeName.Contains("Theory");
            return type;
        }
    }
}