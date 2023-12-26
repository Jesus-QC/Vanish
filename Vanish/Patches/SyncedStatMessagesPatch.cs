using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerStatsSystem;

namespace Vanish.Patches;

[HarmonyPatch(typeof(SyncedStatMessages), nameof(SyncedStatMessages.SendAllStats))]
public class SyncedStatMessagesPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        Label ret = generator.DefineLabel();
        newInstructions[newInstructions.Count - 1].labels.Add(ret);
        
        newInstructions.InsertRange(0, new CodeInstruction[]
        {
            new (OpCodes.Ldarg_1),
            new (OpCodes.Call, AccessTools.Method(typeof(EntryPoint), nameof(EntryPoint.IsVanished))),
            new (OpCodes.Brtrue, ret)
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}