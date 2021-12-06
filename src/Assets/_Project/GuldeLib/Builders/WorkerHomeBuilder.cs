using GuldeLib.Generators;
using GuldeLib.Maps;
using GuldeLib.WorkerHomes;

namespace GuldeLib.Builders
{
    public class WorkerHomeBuilder : Builder<WorkerHome>
    {
        public WorkerHomeBuilder WithLocation(GeneratableLocation location)
        {
            Object.Location = location;
            return this;
        }
    }
}