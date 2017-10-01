﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Lynx.Handler;

namespace Lynx.Interactive
{
    public class Criteria<T> : ICriterion<T>
    {
        private List<ICriterion<T>> _critiera = new List<ICriterion<T>>();

        public Criteria<T> AddCriterion(ICriterion<T> criterion)
        {
            _critiera.Add(criterion);
            return this;
        }

        public async Task<bool> JudgeAsync(LynxContext sourceContext, T parameter)
        {
            foreach (var criterion in _critiera)
            {
                var result = await criterion.JudgeAsync(sourceContext, parameter).ConfigureAwait(false);
                if (!result) return false;
            }
            return true;
        }
    }
}