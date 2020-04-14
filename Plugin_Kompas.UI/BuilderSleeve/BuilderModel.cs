using Kompas6API5;
using SleeveParameters;
using Kompas6Constants3D;

namespace BuilderSleeve
{
    /// <summary>
    /// Класс для построения 3D модели в САПР Компас 3D
    /// </summary>
    public class BuilderModel
    {
        /// <summary>
        /// Хранит ссылку на экземпляр объекта Компас 3D
        /// </summary>
        private KompasObject _kompasObject;

        /// <summary>
        /// Хранит параметры модели втулки
        /// </summary>
        private ModelParameters _modelelParameters;

        /// <summary>
        /// Конструктор класса BuilderModel
        /// </summary>
        /// <param name="parameters">Параметры модели</param>
        /// <param name="kompas">Экземпляр Компас 3D</param>
        public BuilderModel(ModelParameters parameters, KompasObject kompas)
        {
            _modelelParameters = parameters;
            _kompasObject = kompas;
            CreateModel();
        }

        /// <summary>
        /// Построения 3D модели
        /// </summary>
        private void CreateModel()
        {
            ksDocument3D iDocument3D = 
                (ksDocument3D)_kompasObject.Document3D();
            iDocument3D.Create(false, true);
            ksPart iPart = 
                (ksPart)iDocument3D.GetPart((short)Part_Type.pTop_Part);
            CreateOuterRing(iPart);
            CreateMiddleRing(iPart);
            CreateJumper(iPart);
            CreateCentralRing(iPart);
            CutCentralRing(iPart);
        }

        /// <summary>
        /// Построение большого кольца
        /// </summary>
        /// <param name="iPart">Интерфейс детали</param>
        private void CreateOuterRing(ksPart iPart)
        {
            ksEntity planeXOZ =
            (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            ksEntity iSketch =
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_sketch);
            ksSketchDefinition iDefinitionSketch = 
                (ksSketchDefinition)iSketch.GetDefinition();
            iDefinitionSketch.SetPlane(planeXOZ);
            iSketch.Create();
            ksDocument2D iDocument2D = (ksDocument2D)iDefinitionSketch.BeginEdit();
            var radius1 = 
                _modelelParameters.Parameter(ParametersName.OuterRingDiameter1).Value / 2;
            var radius2 = 
                _modelelParameters.Parameter(ParametersName.OuterRingDiameter2).Value / 2;
            iDocument2D.ksCircle(0, 0, radius1, 1);
            iDocument2D.ksCircle(0, 0, radius2, 1);
            iDefinitionSketch.EndEdit();
            var depth = _modelelParameters.Parameter(ParametersName.OuterRingHeight).Value;
            ExctrusionSketch(iPart, iSketch, depth, false);
        }

        /// <summary>
        /// Построение среднего кольца 
        /// </summary>
        /// <param name="iPart">Интерфейс детали</param>
        private void CreateMiddleRing(ksPart iPart)
        {
            ksEntity planeXOZ =
            (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            ksEntity iSketch =
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_sketch);
            ksSketchDefinition iDefinitionSketch = 
                (ksSketchDefinition)iSketch.GetDefinition();
            iDefinitionSketch.SetPlane(planeXOZ);
            iSketch.Create();
            ksDocument2D iDocument2D = (ksDocument2D)iDefinitionSketch.BeginEdit();
            var radius1 = 
                _modelelParameters.Parameter(ParametersName.MiddleRingDiameter1).Value / 2;
            var radius2 = 
                _modelelParameters.Parameter(ParametersName.MiddleRingDiameter2).Value / 2;
            iDocument2D.ksCircle(0, 0, radius1, 1);
            iDocument2D.ksCircle(0, 0, radius2, 1);
            iDefinitionSketch.EndEdit();
            var depth = 
                _modelelParameters.Parameter(ParametersName.MiddleRingHeight).Value;
            ExctrusionSketch(iPart, iSketch, depth, false);
        }

        /// <summary>
        /// Построение перемычек 
        /// </summary>
        private void CreateJumper(ksPart iPart)
        {
            ksEntity planeZOY =
            (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeYOZ);
            ksEntity iSketch =
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_sketch);
            ksSketchDefinition iDefinitionSketch = 
                (ksSketchDefinition)iSketch.GetDefinition();
            iDefinitionSketch.SetPlane(planeZOY);
            iSketch.Create();
            ksDocument2D iDocument2D = (ksDocument2D)iDefinitionSketch.BeginEdit();
            var radius1 = 
                _modelelParameters.Parameter(ParametersName.JumperDiameter).Value / 2;
            var heightCentral = 
                _modelelParameters.Parameter(ParametersName.CentralRingHeight).Value / 2;
            var heightMiddle = 
                _modelelParameters.Parameter(ParametersName.MiddleRingHeight).Value / 2;
            if (heightCentral > heightMiddle)
            {
                iDocument2D.ksCircle(0, heightMiddle, radius1, 1);
            }
            else
            {
                iDocument2D.ksCircle(0, heightCentral, radius1, 1);
            }
            iDefinitionSketch.EndEdit();
            var depth = 
                _modelelParameters.Parameter(ParametersName.MiddleRingDiameter1).Value / 2;
            var extrusion = 
                ExctrusionSketch(iPart, iSketch, depth, false);
            ksEntity entityCircularCopy = 
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_circularCopy);
            ksCircularCopyDefinition ICircularCopyDefinition =
                (ksCircularCopyDefinition)entityCircularCopy.GetDefinition();
            ICircularCopyDefinition.count2 = 
                (int)_modelelParameters.Parameter(ParametersName.JumperNumber).Value;

            ksEntity entityAxis = 
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_axis2Planes);
            ksAxis2PlanesDefinition iAxis2PlanesDefinition =
                entityAxis.GetDefinition();
            ksEntity planeXOY =
                (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOY);
            ksEntity planeYOZ =
                (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeYOZ);

            iAxis2PlanesDefinition.SetPlane(1, planeXOY);
            iAxis2PlanesDefinition.SetPlane(2, planeYOZ);
            entityAxis.Create();
            var numberJamper = 
                (int)_modelelParameters.Parameter(ParametersName.JumperNumber).Value;
            ICircularCopyDefinition.SetCopyParamAlongDir(numberJamper,
                360 / numberJamper, false, false);
            EntityCollection entityCollection =
                 ICircularCopyDefinition.GetOperationArray();
            entityCollection.Clear();
            entityCollection.Add(extrusion);
            ICircularCopyDefinition.SetAxis(entityAxis);
            entityCircularCopy.Create();
        }

        /// <summary>
        /// Построение маленького кольца
        /// </summary>
        /// <param name="iPart">Интерфейс детали</param>
        private void CreateCentralRing(ksPart iPart)
        {
            var depth = 
                _modelelParameters.Parameter(ParametersName.CentralRingHeight).Value;
            ksEntity planeXOZ =
                (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            ksEntity iSketch =
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_sketch);
            ksSketchDefinition iDefinitionSketch = 
                (ksSketchDefinition)iSketch.GetDefinition();
            iDefinitionSketch.SetPlane(planeXOZ);
            iSketch.Create();
            ksDocument2D iDocument2D = (ksDocument2D)iDefinitionSketch.BeginEdit();
            var radius1 = 
                _modelelParameters.Parameter(ParametersName.CentralRingDiameter1).Value / 2;
            var radius2 = 
                _modelelParameters.Parameter(ParametersName.CentralRingDiameter2).Value / 2;
            iDocument2D.ksCircle(0, 0, radius1, 1);
            iDocument2D.ksCircle(0, 0, radius2, 1);
            iDefinitionSketch.EndEdit();
            ExctrusionSketch(iPart, iSketch, depth, false);
        }

        /// <summary>
        /// Вырез маленького кольца
        /// </summary>
        /// <param name="iPart">Интерфейс детали</param>
        private void CutCentralRing(ksPart iPart)
        {
            var depth = 
                _modelelParameters.Parameter(ParametersName.CentralRingHeight).Value;
            ksEntity planeXOZ =
                (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            ksEntity iSketch =
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_sketch);
            ksSketchDefinition iDefinitionSketch = 
                (ksSketchDefinition)iSketch.GetDefinition();
            iDefinitionSketch.SetPlane(planeXOZ);
            iSketch.Create();
            ksDocument2D iDocument2D = 
                (ksDocument2D)iDefinitionSketch.BeginEdit();
            var radius1 = 
                _modelelParameters.Parameter(ParametersName.CentralRingDiameter1).Value / 2;
            iDocument2D.ksCircle(0, 0, radius1, 1);
            iDefinitionSketch.EndEdit();
            CutExctrusionSketch(iPart, iSketch, depth, true);
        }

        /// <summary>
        /// Выдавливание по эскизу
        /// </summary>
        /// <param name="iPart">Интерфейс детали</param>
        /// <param name="iSketch">Эскиз</param>
        /// <param name="depth">Глубина выдавливания</param>
        /// <param name="type">Тип выдавливания</param>
        private ksEntity ExctrusionSketch(ksPart iPart, ksEntity iSketch, double depth, bool type)
        {
            ksEntity entityExtr = 
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_bossExtrusion);
            ksBossExtrusionDefinition extrusionDef =
                (ksBossExtrusionDefinition)entityExtr.GetDefinition();
            ksExtrusionParam extrProp =
                (ksExtrusionParam)extrusionDef.ExtrusionParam();
            extrusionDef.SetSketch(iSketch);
            if (type == false)
            {
                extrProp.direction = (short)Direction_Type.dtReverse;
            }
            if (type == true)
            {
                extrProp.direction = (short)Direction_Type.dtNormal;
            }
            extrProp.typeNormal = (short)End_Type.etBlind;
            if (type == false)
            {
                extrProp.depthReverse = depth;
            }
            if (type == true)
            {
                extrProp.depthNormal = depth;
            }
            entityExtr.Create();
            return entityExtr;
        }

        /// <summary>
        /// Вырезание по эскизу
        /// </summary>
        /// <param name="iPart">Интерфейс детали</param>
        /// <param name="iSketch">Эскиз</param>
        /// <param name="depth">Глубина выдавливания</param>
        /// <param name="type">Тип выдавливания</param>
        private void CutExctrusionSketch(ksPart iPart, ksEntity iSketch, double depth, bool type)
        {
            ksEntity entityExtr = 
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_cutExtrusion);
            ksCutExtrusionDefinition extrusionDef =
                (ksCutExtrusionDefinition)entityExtr.GetDefinition();
            ksExtrusionParam extrProp =
                (ksExtrusionParam)extrusionDef.ExtrusionParam();
            extrusionDef.SetSketch(iSketch);
            if (type == false)
            {
                extrProp.direction = (short)Direction_Type.dtReverse;
            }
            if (type == true)
            {
                extrProp.direction = (short)Direction_Type.dtNormal;
            }
            extrProp.typeNormal = (short)End_Type.etBlind;
            if (type == false)
            {
                extrProp.depthReverse = depth;
            }
            if (type == true)
            {
                extrProp.depthNormal = depth;
            }
            entityExtr.Create();
        }
    }
}
