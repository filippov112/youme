using Pdp.Domain.Interfaces;

namespace Pdp.Domain.Models.Other
{
    public abstract class ComplexBlock(string structure, List<IComponent> components)
    {
        public string Structure { get; set; } = structure;

        public virtual string Build()
        {
            var result = Structure;
            foreach (var component in components)
            {
                string value;
                if (component is ComplexBlock complexComponent)
                    value = complexComponent.Build();
                else
                    value = component.Value;
                result = result.Replace(component.Key, value);
            }
            return result;
        }
    }
}
