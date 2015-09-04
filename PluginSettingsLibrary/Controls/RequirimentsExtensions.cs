using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ADBaseLibrary;

namespace PluginSettingsLibrary.Controls
{
    public static class RequirimentsExtensions
    {

        public static List<Control> GenerateControls(this List<Requirement> requirements, Dictionary<string, object> metadata)
        {

            List<Control> ctrls = new List<Control>();
            foreach (Requirement s in requirements)
            {
                ctrls.Add(s.GetControl(metadata));
            }
            return ctrls;
        }

        public static List<Control> GenerateLabels(this List<Requirement> requirements)
        {
            List<Control> ctrls = new List<Control>();
            foreach (Requirement s in requirements)
            {
                bool islink = (s.RequirementType & ~RequirementType.Required) == RequirementType.Link;
                Label lab = new Label {Anchor = AnchorStyles.Right, Text = islink ? string.Empty : (s.Name + " :"), TextAlign = ContentAlignment.BottomRight,AutoSize = true};
                ctrls.Add(lab);
            }
            return ctrls;
        }

        public static Control GetControl(this Requirement requirement, Dictionary<string, object> meta)
        {
            RequirementType r = (RequirementType)((int)requirement.RequirementType & 0xFF);

            IInit ctrl = null;
            switch (r)
            {
                case RequirementType.Bool:
                    ctrl = new Boolean();
                    break;
                case RequirementType.String:
                    ctrl = new String();
                    break;
                case RequirementType.Integer:
                    ctrl = new Integer();
                    break;
                case RequirementType.Password:
                    ctrl = new Password();
                    break;
                case RequirementType.FilePath:
                    ctrl = new FilePath();
                    break;
                case RequirementType.FolderPath:
                    ctrl = new FolderPath();
                    break;
                case RequirementType.DropDownList:
                    ctrl=new DropDownList();
                    break;
                case RequirementType.Link:
                    ctrl = new Link();
                    break;
            }
            if (ctrl != null)
                ctrl.Init(requirement, meta);
            return (Control) ctrl;
        }
    }
}
