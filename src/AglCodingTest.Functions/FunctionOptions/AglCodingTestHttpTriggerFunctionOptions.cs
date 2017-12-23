using AGLCodeTest.Models;

namespace AGLCodeTest.Functions.FunctionOptions
{
    /// <summary>
    /// This represents the options entity for <see cref="AGLCodeTestHttpTriggerFunction"/>.
    /// </summary>
    public class AGLCodeTestHttpTriggerFunctionOptions : FunctionOptionsBase
    {
        /// <summary>
        /// Gets or sets the <see cref="Models.PetType"/> value.
        /// </summary>
        public PetType PetType { get; set; }
    }
}