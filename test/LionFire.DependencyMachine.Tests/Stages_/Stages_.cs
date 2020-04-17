using LionFire.DependencyMachine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace Stages_
{
    // TODO: Stopping
    // TODO: Instead of Debug messages in DSM.dll, add events, and write to xunit log in this test

    public class Stages_
    {
        [Fact]
        public void P_RandomizedStageMembers()
        {

            var dsm = new DependencyStateMachine() { IsLoggingEnabled = true };


            var r = new Random();

            var stages = new List<string>();
            var bag = new List<int>();
            var stageCount = 30;
            for (int i = 0; i < stageCount; i++)
            {
                stages.Add("stage" + i);
                bag.Add(i);
            }
            dsm.Register(DependencyStages.CreateStageChain(stages.ToArray()));

                var letterCount = 20;

            while (bag.Count > 0)
            {
                var numIndex = r.Next(0, bag.Count - 1);
                var num = bag[numIndex];
                bag.RemoveAt(numIndex);

                Debug.Write($"stage {num}: ");

                var letterBag = new List<char>();
                for (char i = 'a'; i < 'a' + letterCount; i++)
                {
                    letterBag.Add(i);
                }
                while (letterBag.Count > 0)
                {
                    var index = r.Next(0, letterBag.Count );
                    var letter = letterBag[index];
                    letterBag.RemoveAt(index);

                    Debug.Write($"{letter}");
                    dsm.Register(new DependencyContributor($"stage{num}", $"stage{num}-{letter}"));
                }
                Debug.WriteLine("");
            }

            dsm.Start();

            //Assert.Equal(new string[] { "stage1", "stage2", "stage3", "stage4" }, dsm.StartStageLog);
            Assert.Equal(Enumerable.Range(0, stageCount), dsm.StartStageLog);
            Assert.Equal(stageCount * (letterCount + 1), dsm.StartLog.Count());


            Debug.WriteLine(dsm.StartLog);

        }

        [Fact]
        public void P_EmptyStage()
        {
            int emptyStage = 3;

            var dsm = new DependencyStateMachine() { IsLoggingEnabled = true };

            var r = new Random();

            var stages = new List<string>();
            var bag = new List<int>();
            var stageCount = 10;
            for (int i = 0; i < stageCount; i++)
            {
                stages.Add("stage" + i);
                bag.Add(i);
            }
            dsm.Register(DependencyStages.CreateStageChain(stages.ToArray()));

            var letterCount = 6;

            while (bag.Count > 0)
            {
                var numIndex = r.Next(0, bag.Count - 1);
                var num = bag[numIndex];
                bag.RemoveAt(numIndex);

                Debug.Write($"stage {num}: ");

                if (num == emptyStage) continue;

                var letterBag = new List<char>();
                for (char i = 'a'; i < 'a' + letterCount; i++)
                {
                    letterBag.Add(i);
                }
                while (letterBag.Count > 0)
                {
                    var index = r.Next(0, letterBag.Count);
                    var letter = letterBag[index];
                    letterBag.RemoveAt(index);

                    Debug.Write($"{letter}");
                    dsm.Register(new DependencyContributor($"stage{num}", $"stage{num}-{letter}"));
                }
                Debug.WriteLine("");
            }

            dsm.Start();

            //Assert.Equal(new string[] { "stage1", "stage2", "stage3", "stage4" }, dsm.StartStageLog);
            Assert.Equal(Enumerable.Range(0, stageCount), dsm.StartStageLog);
            Assert.Equal((stageCount-1) * (letterCount + 1) + 1, dsm.StartLog.Count());


            Debug.WriteLine(dsm.StartLog);

        }
    }
}
