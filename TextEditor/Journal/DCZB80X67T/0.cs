using System;
namespace Fruit_warehouse
{   /// <summary>
    /// §¬§à§â§à§Ò§Ü§Ñ.
    /// </summary>
    class Box
    {
        // §±§à§Ý§Ö §Õ§Ý§ñ §Ú§Þ§Ö§ß§Ú §Ü§à§â§à§Ò§Ü§Ú.
        private string name;
        // §±§à§Ý§Ö §Õ§Ý§ñ §Ó§Ö§ã§Ñ §Ü§à§â§à§Ò§Ü§Ú.
        private double weight;
        // §±§à§Ý§Ö §Õ§Ý§ñ §è§Ö§ß§í §Ù§Ñ §Ü§Ú§Ý§à§Ô§â§Ñ§Þ§Þ §á§â§à§Õ§å§Ü§ä§Ñ §Ó §Ü§à§â§à§Ò§Ü§Ö.
        private double pricePerKilogram;
        // §±§å§Ò§Ý§Ú§é§ß§à§Ö §ã§Ó§à§Û§ã§ä§Ó§à §Õ§Ý§ñ §Ú§Ù§Þ§Ö§ß§Ö§ß§Ú§ñ §Ú §é§ä§Ö§ß§Ú§ñ §Ú§Þ§Ö§ß§Ú.
        public string Name
        {
            get => name; set => name = value;
        }
        // §±§å§Ò§Ý§Ú§é§ß§à§Ö §ã§Ó§à§Û§ã§ä§Ó§à §Õ§Ý§ñ §é§ä§Ö§ß§Ú§ñ §Ó§Ö§ã§Ñ.
        public double Weight { get => weight; }

        // §±§å§Ò§Ý§Ú§é§ß§à§Ö §ã§Ó§à§Û§ã§ä§Ó§à §Õ§Ý§ñ §Ú§Ù§Þ§Ö§ß§Ö§ß§Ú§ñ §Ú §é§ä§Ö§ß§Ú§ñ §è§Ö§ß§í.
        public double PricePerKilogram
        {
            get => pricePerKilogram; set => pricePerKilogram = value;
        }

        // §¬§à§ß§ã§ä§â§å§Ü§ä§à§â, §á§à§Ý§å§é§Ñ§ð§ë§Ú§Û §ß§Ñ §Ó§ç§à§Õ §Ù§ß§Ñ§é§Ö§ß§Ú§ñ §Ó§Ö§ã§Ñ §Ú §è§Ö§ß§í.
        public Box(double weight, double pricePerKilogram)
        {
            this.weight = weight;
            this.pricePerKilogram = pricePerKilogram;
        }

        /// <summary>
        /// §±§Ö§â§Ö§à§á§â§Ö§Õ§Ö§Ý§×§ß§ß§í§Û §Þ§Ö§ä§à§Õ, §Ü§à§ä§à§â§í§Û §Ó§à§Ù§Ó§â§Ñ§ë§Ñ§Ö§ä §ã§ä§â§à§Ü§å §ã §á§à§Ý§ß§í§Þ §à§á§Ú§ã§Ñ§ß§Ú§Ö§Þ §Ü§à§â§à§Ò§Ü§Ú.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{name}. §®§Ñ§ã§ã§Ñ : {weight}. §¸§Ö§ß§Ñ §Ù§Ñ §Ü§Ú§Ý§à§Ô§â§Ñ§Þ§Þ : {pricePerKilogram} §â. §°§Ò§ë§Ñ§ñ§ñ §è§Ö§ß§ß§à§ã§ä§î (§Ò§Ö§Ù §å§é§Ö§ä§Ñ §ã§ä§Ö§á§Ö§ß§Ú §á§à§Ó§â§Ö§Ø§Õ§Ö§ß§Ú§ñ §Ü§à§ß§ä§Ö§Û§ß§Ö§â§Ñ) : {Math.Round(weight * pricePerKilogram, 2)} §â.";
        }
    }
}
