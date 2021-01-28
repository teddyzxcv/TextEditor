using System;
namespace Fruit_warehouse
{
    /// <summary>
    /// ������ҧܧ�.
    /// </summary>
    class Box
    {
        // ����ݧ� �էݧ� �ڧާ֧ߧ� �ܧ���ҧܧ�.
        private string name;
        // ����ݧ� �էݧ� �ӧ֧�� �ܧ���ҧܧ�.
        private double weight;
        // ����ݧ� �էݧ� ��֧ߧ� �٧� �ܧڧݧ�ԧ�ѧާ� ����է�ܧ�� �� �ܧ���ҧܧ�.
        private double pricePerKilogram;
        // ����ҧݧڧ�ߧ�� ��ӧ�ۧ��ӧ� �էݧ� �ڧ٧ާ֧ߧ֧ߧڧ� �� ���֧ߧڧ� �ڧާ֧ߧ�.
        public string Name
        {
            get => name; set => name = value;
        }
        // ����ҧݧڧ�ߧ�� ��ӧ�ۧ��ӧ� �էݧ� ���֧ߧڧ� �ӧ֧��.
        public double Weight { get => weight; }

        // ����ҧݧڧ�ߧ�� ��ӧ�ۧ��ӧ� �էݧ� �ڧ٧ާ֧ߧ֧ߧڧ� �� ���֧ߧڧ� ��֧ߧ�.
        public double PricePerKilogram
        {
            get => pricePerKilogram; set => pricePerKilogram = value;
        }

        // ����ߧ����ܧ���, ���ݧ��ѧ��ڧ� �ߧ� �ӧ��� �٧ߧѧ�֧ߧڧ� �ӧ֧�� �� ��֧ߧ�.
        public Box(double weight, double pricePerKilogram)
        {
            this.weight = weight;
            this.pricePerKilogram = pricePerKilogram;
        }

        /// <summary>
        /// ���֧�֧���֧է֧ݧקߧߧ��� �ާ֧���, �ܧ������� �ӧ�٧ӧ�ѧ�ѧ֧� �����ܧ� �� ���ݧߧ��� ���ڧ�ѧߧڧ֧� �ܧ���ҧܧ�.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{name}. ���ѧ��� : {weight}. ���֧ߧ� �٧� �ܧڧݧ�ԧ�ѧާ� : {pricePerKilogram} ��. ���ҧ�ѧ�� ��֧ߧߧ���� (�ҧ֧� ���֧�� ���֧�֧ߧ� ���ӧ�֧اէ֧ߧڧ� �ܧ�ߧ�֧ۧߧ֧��) : {Math.Round(weight * pricePerKilogram, 2)} ��.";
        }
    }
}
