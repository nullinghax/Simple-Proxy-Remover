using System;
using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System.Text;
using System.Collections.Generic;


namespace Proxy_Remover
{
    internal class Remove
    {
        public static void Run(ModuleDefMD module)
        {
            int fixedProxies = 0;
            foreach (TypeDef type in module.Types)
            {
                List<MethodDef> methodsToDelete = new List<MethodDef>();
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i] != null && method.Body.Instructions[i].OpCode == OpCodes.Call)
                        {
                            try
                            {
                                MethodDef methodDef = (MethodDef)method.Body.Instructions[i].Operand;
                                for (int instr = 0; instr < methodDef.Body.Instructions.Count; instr++)
                                {
                                    if(methodDef.Body.Instructions[instr].OpCode == OpCodes.Ldc_I4 && methodDef.Body.Instructions[instr+1].OpCode == OpCodes.Stloc_0)
                                    {
                                        methodsToDelete.Add(methodDef);
                                        fixedProxies++;
                                        method.Body.Instructions[i].OpCode = OpCodes.Nop;
                                        method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(methodDef.Body.Instructions[instr].GetLdcI4Value()));
                                        
                                    }
                                    if (methodDef.Body.Instructions[instr].OpCode == OpCodes.Ldstr && methodDef.Body.Instructions[instr + 1].OpCode == OpCodes.Stloc_0)
                                    {
                                        methodsToDelete.Add(methodDef);
                                        fixedProxies++;
                                        method.Body.Instructions[i].OpCode = OpCodes.Nop;
                                        method.Body.Instructions.Insert(i + 1, OpCodes.Ldstr.ToInstruction(methodDef.Body.Instructions[instr].Operand.ToString()));
                                        
                                    }
                                    if (methodDef.Body.Instructions[instr].OpCode == OpCodes.Ldc_I4_0 || methodDef.Body.Instructions[instr].OpCode == OpCodes.Ldc_I4_1 && methodDef.Body.Instructions[instr + 1].OpCode == OpCodes.Stloc_0)
                                    {
                                        methodsToDelete.Add(methodDef);
                                        fixedProxies++;
                                        method.Body.Instructions[i].OpCode = OpCodes.Nop;
                                        if(methodDef.Body.Instructions[instr].OpCode == OpCodes.Ldc_I4_0)
                                        {
                                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4_0.ToInstruction());
                                        }
                                        else if (methodDef.Body.Instructions[instr].OpCode == OpCodes.Ldc_I4_1)
                                        {
                                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4_1.ToInstruction());
                                        }
                                        
                                    }
                                    
                                }
                            } catch { }
                        }
                    }
                    

                }
                foreach (var methods in methodsToDelete)
                    type.Methods.Remove(methods);
            }
            Console.WriteLine($"Removed {fixedProxies} Proxies");
        }
    }
}
