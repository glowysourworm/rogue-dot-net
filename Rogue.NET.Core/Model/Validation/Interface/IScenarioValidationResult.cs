using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Validation.Interface
{
    public interface IScenarioValidationResult
    {
        string InnerMessage { get; set; }
        bool Passed { get; set; }
    }
}
