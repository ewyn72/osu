// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using osu.Game.Online.API;
using osu.Game.Rulesets.Osu;

namespace osu.Game.Benchmarks
{
    public class BenchmarkRuleset : BenchmarkTest
    {
        private OsuRuleset ruleset;
        private APIMod apiModDoubleTime;
        private APIMod apiModDifficultyAdjust;

        public override void SetUp()
        {
            base.SetUp();
            ruleset = new OsuRuleset();
            apiModDoubleTime = new APIMod { Acronym = "DT" };
            apiModDifficultyAdjust = new APIMod { Acronym = "DA" };
        }

        [Benchmark]
        public void BenchmarkToModDoubleTime()
        {
            apiModDoubleTime.ToMod(ruleset);
        }

        [Benchmark]
        public void BenchmarkToModDifficultyAdjust()
        {
            apiModDifficultyAdjust.ToMod(ruleset);
        }

        [Benchmark]
        public void BenchmarkGetAllMods()
        {
            ruleset.GetAllMods().Consume(new Consumer());
        }
    }
}
