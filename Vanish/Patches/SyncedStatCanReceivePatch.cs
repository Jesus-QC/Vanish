using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerStatsSystem;

namespace Vanish.Patches;

[HarmonyPatch(typeof(SyncedStatBase), nameof(SyncedStatBase.CanReceive))]
public class SyncedStatCanReceivePatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        Label ret = generator.DefineLabel();
        newInstructions[newInstructions.Count - 2].labels.Add(ret);
        
        newInstructions.InsertRange(0, new CodeInstruction[]
        {
            new (OpCodes.Ldarg_0),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(SyncedStatBase), nameof(SyncedStatBase.Hub))),
            new (OpCodes.Call, AccessTools.Method(typeof(EntryPoint), nameof(EntryPoint.IsVanished))),
            new (OpCodes.Brtrue, ret)
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}