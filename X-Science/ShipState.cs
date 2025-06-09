using KSP.Localization;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using System.Reflection;
using KSP.UI.Screens;



namespace ScienceChecklist
{
    public enum Statuses { none, seat, pod }
    class ShipStateWindow : Window<ScienceChecklistAddon>
    {
        private GUIStyle labelStyle;
        private GUIStyle sectionStyle;
        private Vector2 scrollPosition;
        private readonly ScienceChecklistAddon _parent;
        private readonly Logger _logger;
        private MapObject SelectedObject;
        public Statuses status = Statuses.none;


        public ShipStateWindow(ScienceChecklistAddon Parent)
            : base(Localizer.Format("#LOC_xSci_193"), 250, 30)
        {
            _parent = Parent;
            UiScale = ScienceChecklistAddon.Config.UiScale;
            scrollPosition = Vector2.zero;
            ScienceChecklistAddon.Config.UiScaleChanged += OnUiScaleChange;
            _logger = new Logger(this);
            SelectedObject = null;

            _parent.ScienceEventHandler.MapObjectSelected += (s, e) => MapObjectSelected(s, e);
        }



        protected override void ConfigureStyles()
        {
            base.ConfigureStyles();

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(_skin.label);
                labelStyle.wordWrap = true;
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.normal.textColor = Color.white;
                labelStyle.stretchWidth = true;
                labelStyle.stretchHeight = false;
                labelStyle.margin.bottom -= wScale(2);
                labelStyle.padding.bottom -= wScale(2);
            }

            if (sectionStyle == null)
            {
                sectionStyle = new GUIStyle(labelStyle);
                sectionStyle.fontStyle = FontStyle.Bold;
            }
        }



        private void OnUiScaleChange(object sender, EventArgs e)
        {
            UiScale = ScienceChecklistAddon.Config.UiScale;
            labelStyle = null;
            sectionStyle = null;
            base.OnUiScaleChange();
            ConfigureStyles();
        }



        protected override void DrawWindowContents(int windowID)
        {
            switch (SelectedObject.type)
            {
                case MapObject.ObjectType.CelestialBody:
                    DrawBody();
                    break;
                case MapObject.ObjectType.Vessel:
                    DrawVessel();
                    break;
            }
        }

        protected void DrawBody()
        {
            string Title = "";
            string Text = "";
            if (SelectedObject.celestialBody)
            {
                Body Body = _parent.Science.BodyList[SelectedObject.celestialBody];
                Title += Localizer.Format("#LOC_xSci_194") + GameHelper.LocalizeBodyName(Body.CelestialBody.displayName) + "\n";
                Title += Body.Type;
                if (Body.IsHome)
                    Title += Localizer.Format("#LOC_xSci_195");



                Text += Localizer.Format("#LOC_xSci_196") + (Body.CelestialBody.scienceValues.spaceAltitudeThreshold / 1000) + Localizer.Format("#LOC_xSci_197");
                if (Body.HasAtmosphere)
                {
                    Text += Localizer.Format("#LOC_xSci_198") + (Body.CelestialBody.atmosphereDepth / 1000) + Localizer.Format("#LOC_xSci_197");
                    Text += Localizer.Format("#LOC_xSci_199") + (Body.CelestialBody.scienceValues.flyingAltitudeThreshold / 1000) + Localizer.Format("#LOC_xSci_197");
                    if (Body.HasOxygen)
                        Text += Localizer.Format("#LOC_xSci_200");
                }
                else
                    Text += Localizer.Format("#LOC_xSci_201");

                if (Body.HasSurface)
                {
                    if (Body.HasOcean)
                        Text += Localizer.Format("#LOC_xSci_202");
                }
                else
                    Text += Localizer.Format("#LOC_xSci_203");
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            GUILayout.Label(Title, sectionStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(wScale(16));
            GUILayout.Label(Text, labelStyle, GUILayout.ExpandWidth(true));

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(wScale(8));
        }



        protected void DrawVessel()
        {
            string Title = "";
            string Text = "";
            if (SelectedObject.vessel != null && SelectedObject.vessel.protoVessel != null)
            {
                if (SelectedObject.vessel.DiscoveryInfo.Level != DiscoveryLevels.Owned)
                    Title = Localizer.Format("#LOC_xSci_204");
                else
                {
                    Title = SelectedObject.vessel.GetName();
                    ProtoVessel proto = SelectedObject.vessel.protoVessel;
                    double mass = 0;
                    var res = new SortedDictionary<string, xResourceData>();
                    foreach (ProtoPartSnapshot p in proto.protoPartSnapshots)
                    {
                        foreach (var r in p.resources)
                        {
                            xResourceData d;
                            if (res.ContainsKey(r.resourceName))
                                d = res[r.resourceName];
                            else
                            {
                                d = new xResourceData(r.resourceName);
                            }
                            d.current += r.amount;
                            d.max += r.maxAmount;
                            res[r.resourceName] = d;
                        }
                        mass += p.mass;
                        CheckCommand(p);
                    }



                    var texts = res.Values.ToList().ConvertAll(d => d.ToString());

                    if (!SelectedObject.vessel.isEVA)
                    {
                        texts.Add("");
                        var crew = proto.GetVesselCrew().Count();
                        mass += res.Values.Sum(d => d.GetMass());
                        var parts = proto.protoPartSnapshots.Count();
                        texts.Add(string.Format(Localizer.Format("#LOC_xSci_205"), crew, parts, mass));



                        switch (this.status)
                        {
                            case Statuses.pod:
                                break;
                            case Statuses.none:
                                texts.Add(Localizer.Format("#LOC_xSci_206"));
                                break;
                            case Statuses.seat:
                                texts.Add(Localizer.Format("#LOC_xSci_207"));
                                break;
                        }
                    }

                    Text = string.Join("\n", texts.ToArray());

                }



            }





            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            GUILayout.Label(Title, sectionStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(wScale(16));
            GUILayout.Label(Text, labelStyle, GUILayout.ExpandWidth(true));

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(wScale(8));
        }


        public void CheckCommand(ProtoPartSnapshot part)
        {
            if (status == Statuses.pod)
                return;

            foreach (var m in part.modules)
            {
                if (m.moduleName == Localizer.Format("#LOC_xSci_208"))
                {
                    status = Statuses.pod;
                    return;
                }
                if (m.moduleName == Localizer.Format("#LOC_xSci_209"))
                {
                    status = Statuses.seat;
                }
            }
        }


        public void MapObjectSelected(object sender, NewSelectionData SelectionData)
        {
            //_logger.Trace( "MapObjectSelected" );
            //_logger.Trace( SelectionData._selectedObject.type.ToString( ) );

            if (!ScienceChecklistAddon.Config.SelectedObjectWindow)
                return;

            switch (SelectionData._selectedObject.type)
            {
                case MapObject.ObjectType.CelestialBody:
                    SetVisible(true);
                    SelectedObject = SelectionData._selectedObject;
                    if (!_parent.Science.BodyList.ContainsKey(SelectedObject.celestialBody))
                        _parent.Science.Reset();
                    break;
                case MapObject.ObjectType.Vessel:
                    SetVisible(true);
                    SelectedObject = SelectionData._selectedObject;
                    break;
                default:
                    SetVisible(false);
                    break;
            }

        }



        public WindowSettings BuildSettings()
        {
            //_logger.Info( "BuildSettings" );
            WindowSettings W = new WindowSettings(ScienceChecklistAddon.WINDOW_NAME_SHIP_STATE);
            W.Set(Localizer.Format("#LOC_xSci_135"), (int)windowPos.yMin);
            W.Set(Localizer.Format("#LOC_xSci_136"), (int)windowPos.xMin);
            W.Set(Localizer.Format("#LOC_xSci_210"), (int)windowPos.width);
            W.Set(Localizer.Format("#LOC_xSci_211"), (int)windowPos.height);

            return W;
        }



        public void ApplySettings(WindowSettings W)
        {
            windowPos.yMin = W.GetInt(Localizer.Format("#LOC_xSci_135"), 40);
            windowPos.xMin = W.GetInt(Localizer.Format("#LOC_xSci_136"), 40);
            windowPos.width = W.GetInt(Localizer.Format("#LOC_xSci_210"), 200);
            windowPos.height = W.GetInt(Localizer.Format("#LOC_xSci_211"), 200);

            if (windowPos.width < 100)
                windowPos.width = 100;

            if (windowPos.width < 50)
                windowPos.width = 50;

        }
    }



    class xResourceData
    {
        public double current, max;

        public readonly string name;
        readonly PartResourceDefinition def;

        public xResourceData(string name)
        {
            this.name = name;
            this.def = PartResourceLibrary.Instance.GetDefinition(name);
        }

        public double GetMass()
        {
            return def == null ? 0 : def.density * current;
        }

        #region NO_LOCALIZATION
        public override string ToString()
        {
            return string.Format("{0}: {1} / {2}", name, s(current), s(max));
        }
        #endregion

        private static string s(double d)
        {
            #region NO_LOCALIZATION
            return d.ToString(d < 100 ? "f2" : "f0");
            #endregion
        }
    }









}
