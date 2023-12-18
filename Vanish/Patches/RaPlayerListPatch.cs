using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;
using RemoteAdmin.Communication;

namespace Vanish.Patches;

[HarmonyPatch(typeof(RaPlayerList), nameof(RaPlayerList.ReceiveData), new []{typeof(CommandSender), typeof(string)})]
public class RaPlayerListPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Beq) + 1;
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new (OpCodes.Ldloc_S, 9),
            new (OpCodes.Call, AccessTools.Method(typeof(RaPlayerListPatch), nameof(IsVanished))),
            new (OpCodes.Brtrue, newInstructions[index - 1].operand)
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    private static bool IsVanished(ReferenceHub hub) => EntryPoint.VanishedPlayers.Contains(hub);
}