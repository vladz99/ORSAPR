using Kompas6API5;
using SleeveParameters;
using Kompas6Constants3D;

namespace BuilderSleeve
{
    /// <summary>
    /// Класс для построения 3Д модели в САПР Компас 3Д
    /// </summary>
    public class BuilderModel
    {
        /// <summary>
        /// Хранит ссылку на экземпляр объекта Компас 3Д
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
        /// <param name="kompas">Экзепляр Компас 3Д</param>
        public BuilderModel(ModelParameters parameters, KompasObject kompas)
        {
            _modelelParameters = parameters;
            _kompasObject = kompas;
            CreateModel();
        }

        /// <summary>
        /// Построения 3Д модели
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
        }

        private double Parameter(ParametersName name)
        {
            return _modelelParameters.
                Parameter(name).Value;
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
            var radius1 = Parameter(ParametersName.OuterRingDiameter1) / 2;
            var radius2 = Parameter(ParametersName.OuterRingDiameter2) / 2;
            iDocument2D.ksCircle(0, 0, radius1, 1);
            iDocument2D.ksCircle(0, 0, radius2, 1);
            iDefinitionSketch.EndEdit();
            var depth = Parameter(ParametersName.OuterRingHeight);
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
            var radius1 = Parameter(ParametersName.MiddleRingDiameter1) / 2;
            var radius2 = Parameter(ParametersName.MiddleRingDiameter2) / 2;
            iDocument2D.ksCircle(0, 0, radius1, 1);
            iDocument2D.ksCircle(0, 0, radius2, 1);
            iDefinitionSketch.EndEdit();
            var depth = Parameter(ParametersName.MiddleRingHeight);
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
            var radius1 = Parameter(ParametersName.JumperDiameter) / 2;
            var heightCentral = Parameter(ParametersName.CentralRingHeight) / 2;
            var heightMiddle = Parameter(ParametersName.MiddleRingHeight) / 2;
            if (heightCentral > heightMiddle)
            {
                iDocument2D.ksCircle(0, heightMiddle, radius1, 1);
            }
            else
            {
                iDocument2D.ksCircle(0, heightCentral, radius1, 1);
            }
            iDefinitionSketch.EndEdit();
            var depth = Parameter(ParametersName.MiddleRingDiameter1) / 2;
            var extrusion = 
                ExctrusionSketch(iPart, iSketch, depth, false);
            ksEntity entityCircularCopy = 
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_circularCopy);
            ksCircularCopyDefinition ICircularCopyDefinition =
                (ksCircularCopyDefinition)entityCircularCopy.GetDefinition();
            ICircularCopyDefinition.count2 =
                (int)Parameter(ParametersName.JumperNumber);

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
                (int)Parameter(ParametersName.JumperNumber);
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
            var depth = Parameter(ParametersName.CentralRingHeight);
            ksEntity planeXOZ =
                (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            ksEntity iSketch =
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_sketch);
            ksSketchDefinition iDefinitionSketch = 
                (ksSketchDefinition)iSketch.GetDefinition();
            iDefinitionSketch.SetPlane(planeXOZ);
            iSketch.Create();
            ksDocument2D iDocument2D = (ksDocument2D)iDefinitionSketch.BeginEdit();
            var radius1 = Parameter(ParametersName.CentralRingDiameter1) / 2;
            var radius2 = Parameter(ParametersName.CentralRingDiameter2) / 2;
            iDocument2D.ksCircle(0, 0, radius1, 1);
            iDocument2D.ksCircle(0, 0, radius2, 1);
            iDefinitionSketch.EndEdit();
            ExctrusionSketch(iPart, iSketch, depth, false);
            CutCentralRing(iPart);
            CreateThread(iPart);
        }

        /// <summary>
        /// Создание резьбы во внутренней части маленького кольца
        /// </summary>
        /// <param name="iPart">Интерфейс детали</param>
        private void CreateThread(ksPart iPart)
        {
            ksEntity planeXOY =
                (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOY);
            ksEntity iSketch =
                (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_sketch);
            ksSketchDefinition iDefinitionSketch =
                (ksSketchDefinition)iSketch.GetDefinition();
            iDefinitionSketch.SetPlane(planeXOY);
            iSketch.Create();
            ksDocument2D iDocument2D = (ksDocument2D)iDefinitionSketch.BeginEdit();
            var X = Parameter(ParametersName.CentralRingDiameter1) / 2;
            var Y = 0;
            if (_modelelParameters.ThreadTypeValue() != ThreadType.NoneThread)
            {
                if(_modelelParameters.ThreadTypeValue() == ThreadType.MetricThread)
                {
                    iDocument2D.ksLineSeg(X - 3, Y - 0.5, X + 2, Y, 1);
                    iDocument2D.ksLineSeg(X - 3, Y + 0.5, X + 2, Y, 1);
                    iDocument2D.ksLineSeg(X - 3, Y - 0.5, X - 3, Y + 0.5, 1);
                }
                if(_modelelParameters.ThreadTypeValue() == ThreadType.ThrustThread)
                {
                    iDocument2D.ksLineSeg(X - 3, Y, X + 2, Y, 1);
                    iDocument2D.ksLineSeg(X - 3, Y + 1, X + 2, Y, 1);
                    iDocument2D.ksLineSeg(X - 3, Y, X - 3, Y + 1, 1);
                }
                iDefinitionSketch.EndEdit();

                ksEntity conicSpiral =
                    (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_cylindricSpiral);
                ksCylindricSpiralDefinition iCylindricSpiralDefinition =
                    (ksCylindricSpiralDefinition)conicSpiral.GetDefinition();
                iCylindricSpiralDefinition.diamType = 0;
                iCylindricSpiralDefinition.buildDir = false;
                iCylindricSpiralDefinition.diam = Parameter(ParametersName.CentralRingDiameter1);
                iCylindricSpiralDefinition.buildMode = 2;
                iCylindricSpiralDefinition.turn = 20;
                iCylindricSpiralDefinition.height = Parameter(ParametersName.CentralRingHeight);

                ksEntityCollection entityCollectionPart =
                    (ksEntityCollection)iPart.EntityCollection((short)Obj3dType.o3d_face);
                ksEntity planeXOZ =
                    (ksEntity)iPart.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
                iCylindricSpiralDefinition.SetPlane(planeXOZ);
                conicSpiral.useColor = 00000;
                conicSpiral.Create();

                ksEntity cinematicEvolition =
                    (ksEntity)iPart.NewEntity((short)Obj3dType.o3d_cutEvolution);
                ksCutEvolutionDefinition CutEvolutionDefinition =
                    (ksCutEvolutionDefinition)cinematicEvolition.GetDefinition();
                CutEvolutionDefinition.SetSketch(iSketch);
                ksEntityCollection collection = 
                    (ksEntityCollection)CutEvolutionDefinition.PathPartArray();
                collection.Add(conicSpiral);
                cinematicEvolition.Create();
            }
        }

        /// <summary>
        /// Вырез маленького кольца
        /// </summary>
        /// <param name="iPart">Интерфейс детали</param>
        private void CutCentralRing(ksPart iPart)
        {
            var depth = Parameter(ParametersName.CentralRingHeight);
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
            var radius1 = Parameter(ParametersName.CentralRingDiameter1) / 2;
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
