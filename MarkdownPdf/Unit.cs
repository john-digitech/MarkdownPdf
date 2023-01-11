using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownPdf
{
    public class Unit
    {
        private MigraDocCore.DocumentObjectModel.Unit _unit;

        public UnitType UnitType
        {
            get
            {
                return FromMigraDoc(_unit.Type);
            }
            set
            {
                _unit.ConvertType(ToMigraDoc(value));
            }
        }

        public double Value
        {
            get => _unit.Value;
            set => _unit.Value = value;
        }

        internal Unit(MigraDocCore.DocumentObjectModel.Unit unit)
        {
            _unit = unit;
        }

        public Unit()
        {
        }

        public Unit(double value, UnitType unitType)
        {
            _unit = new MigraDocCore.DocumentObjectModel.Unit(value, ToMigraDoc(unitType));
        }

        public static implicit operator MigraDocCore.DocumentObjectModel.Unit(Unit unit)
        {
            return unit._unit;
        }

        public static implicit operator Unit(MigraDocCore.DocumentObjectModel.Unit unit)
        {
            return new Unit(unit);
        }

        public static Unit FromCentimeter(double value)
        {
            return new Unit(value, UnitType.Centimeter);
        }

        public static Unit FromMillimeter(double value)
        {
            return new Unit(value, UnitType.Millimeter);
        }

        public static Unit FromInch(double value)
        {
            return new Unit(value, UnitType.Inch);
        }

        public static Unit FromPoint(double value)
        {
            return new Unit(value, UnitType.Point);
        }

        public static Unit FromPica(double value)
        {
            return new Unit(value, UnitType.Pica);
        }

        private MigraDocCore.DocumentObjectModel.UnitType ToMigraDoc(UnitType unitType)
        {
            switch (unitType)
            {
                case UnitType.Centimeter:
                    return MigraDocCore.DocumentObjectModel.UnitType.Centimeter;
                case UnitType.Inch:
                    return MigraDocCore.DocumentObjectModel.UnitType.Inch;
                case UnitType.Millimeter:
                    return MigraDocCore.DocumentObjectModel.UnitType.Millimeter;
                case UnitType.Pica:
                    return MigraDocCore.DocumentObjectModel.UnitType.Pica;
                case UnitType.Point:
                    return MigraDocCore.DocumentObjectModel.UnitType.Point;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }

        private UnitType FromMigraDoc(MigraDocCore.DocumentObjectModel.UnitType unitType)
        {
            switch (unitType)
            {
                case MigraDocCore.DocumentObjectModel.UnitType.Centimeter:
                    return UnitType.Centimeter;
                case MigraDocCore.DocumentObjectModel.UnitType.Inch:
                    return UnitType.Inch;
                case MigraDocCore.DocumentObjectModel.UnitType.Millimeter:
                    return UnitType.Millimeter;
                case MigraDocCore.DocumentObjectModel.UnitType.Pica:
                    return UnitType.Pica;
                case MigraDocCore.DocumentObjectModel.UnitType.Point:
                    return UnitType.Point;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }
    }

    public enum UnitType
    {
        Centimeter,
        Inch,
        Millimeter,
        Point,
        Pica,
    }
}
