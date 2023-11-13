using Raskroi.Calculators.MaterialRequirements;
using Raskroi.Models;

namespace Raskroi.Calculators.Sheating.Algorithms.MinimumWaste
{
    /// <summary>
    /// Алгоритм расчета минимального остатка,
    /// сортируем по потребности, 
    /// </summary>
    public class MinimumWasteAlgorithm : ILinearMaterialOptimizeAlgorithms
    {
        public IEnumerable<Panel> Calculate(IEnumerable<LinearMaterialRequirement> materialRequirements)
        {
            if (materialRequirements == null)
            {
                throw new ArgumentNullException(nameof(materialRequirements));
            }

            throw new NotImplementedException();
        }
    }
}
