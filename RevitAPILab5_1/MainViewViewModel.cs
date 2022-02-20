using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPILab5_1
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SelectedCommandPipe { get; }
        public DelegateCommand SelectedCommandWall { get; }
        public DelegateCommand SelectedCommandDoor { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SelectedCommandPipe = new DelegateCommand(OnSelectPipe);
            SelectedCommandWall = new DelegateCommand(OnSelectWall);
            SelectedCommandDoor = new DelegateCommand(OnSelectDoor);
        }

        public event EventHandler HideRequest;

        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;

        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectDoor()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<FamilyInstance> familyInstances = new FilteredElementCollector(doc)
               .OfCategory(BuiltInCategory.OST_Doors)
               .WhereElementIsNotElementType()
               .Cast<FamilyInstance>()
               .ToList();

            TaskDialog.Show("Количество дверей", $"Количество дверей: {familyInstances.Count.ToString()}");

            RaiseShowRequest();
        }

        private void OnSelectWall()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var walls = new FilteredElementCollector(doc)
               .OfClass(typeof(Wall))
               .Cast<Wall>()
               .ToList();

            double Sum = 0;
            foreach (var wall in walls)
            {
                Parameter volumeParameter = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (volumeParameter.StorageType == StorageType.Double)
                {
                    double volumeValue = UnitUtils.ConvertFromInternalUnits(volumeParameter.AsDouble(), UnitTypeId.CubicMeters);
                    Sum += Math.Round(volumeValue, 2);
                }
            }

            TaskDialog.Show("Объем стен", $"Объем стен: {Sum} м³");

            RaiseShowRequest();
        }

        private void OnSelectPipe()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipes = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .Cast<Pipe>()
                .ToList();

            TaskDialog.Show("Количество труб", $"Количество труб: {pipes.Count.ToString()}");

            RaiseShowRequest();
        }
    }
}
