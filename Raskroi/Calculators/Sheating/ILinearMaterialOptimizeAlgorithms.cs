using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raskroi.Calculators.MaterialRequirements;
using Raskroi.Models;

namespace Raskroi.Calculators.Sheating
{
    public interface ILinearMaterialOptimizeAlgorithms
    {
        IEnumerable<Panel> Calculate(IEnumerable<LinearMaterialRequirement> materialRequirements);
    }
}
