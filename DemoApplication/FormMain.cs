using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YAXLib;
using DemoApplication.SampleClasses;
using System.Xml.Linq;
using System.Collections;
using System.Threading;

namespace DemoApplication
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            InitListOfClasses();
            InitComboBoxes();
        }

        private void InitComboBoxes()
        {
            comboPolicy.Items.AddRange(Enum.GetNames(typeof(YAXExceptionHandlingPolicies)));
            comboErrorType.Items.AddRange(Enum.GetNames(typeof(YAXExceptionTypes)));
            comboOptions.Items.AddRange(Enum.GetNames(typeof(YAXSerializationOptions)));

            if (comboPolicy.Items.Count > 0)
                comboPolicy.Text = YAXExceptionHandlingPolicies.DoNotThrow.ToString();
            if(comboErrorType.Items.Count > 0)
                comboErrorType.Text = YAXExceptionTypes.Error.ToString();
            if (comboOptions.Items.Count > 0)
                comboOptions.Text = YAXSerializationOptions.SerializeNullObjects.ToString();
        }

        private YAXExceptionTypes GetSelectedDefaultExceptionType()
        {
            return (YAXExceptionTypes)Enum.Parse(typeof(YAXExceptionTypes), comboErrorType.Text);
        }

        private YAXExceptionHandlingPolicies GetSelectedExceptionHandlingPolicy()
        {
            return (YAXExceptionHandlingPolicies)Enum.Parse(typeof(YAXExceptionHandlingPolicies), comboPolicy.Text);
        }

        private YAXSerializationOptions GetSelectedSerializationOption()
        {
            return (YAXSerializationOptions)Enum.Parse(typeof(YAXSerializationOptions), comboOptions.Text);
        }

        private void InitListOfClasses()
        {
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(Book), Book.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(BookStruct), BookStruct.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(WarehouseSimple), WarehouseSimple.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(WarehouseStructured), WarehouseStructured.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(WarehouseWithArray), WarehouseWithArray.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(WarehouseWithDictionary), WarehouseWithDictionary.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(WarehouseNestedObjectExample), WarehouseNestedObjectExample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(ProgrammingLanguage), ProgrammingLanguage.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(ColorExample), ColorExample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(MultilevelClass), MultilevelClass.GetSampleInstance()));

            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(FormattingExample), FormattingExample.GetSampleInstance()));

            
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(PathsExample), PathsExample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(MoreComplexExample), MoreComplexExample.GetSampleInstance()));

            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(NestedDicSample), NestedDicSample.GetSampleInstance()));

            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(GUIDTest), GUIDTest.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(TimeSpanSample), TimeSpanSample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(NullableClass), NullableClass.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(NullableSample2), NullableSample2.GetSampleInstance()));
            //lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(ListHolderClass), ListHolderClass.GetSampleInstance()));
            //lstSampleClasses.Items.Add(new ClassInfoListItem(ListHolderClass.GetSampleInstance().ListOfStrings.GetType(), ListHolderClass.GetSampleInstance().ListOfStrings));
            //lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(NamesExample), NamesExample.GetSampleInstance()));
            //lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(Request), Request.GetSampleInstance()));

            //lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(AudioSample), AudioSample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(FieldSerializationExample), FieldSerializationExample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(MoreComplexBook), MoreComplexBook.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(MoreComplexBook2), MoreComplexBook2.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(MoreComplexBook3), MoreComplexBook3.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(WarehouseWithDictionaryNoContainer), WarehouseWithDictionaryNoContainer.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(WarehouseWithComments), WarehouseWithComments.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(EnumsSample), EnumsSample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(MultiDimArraySample), MultiDimArraySample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(AnotherArraySample), AnotherArraySample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(CollectionOfInterfacesSample), CollectionOfInterfacesSample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(InterfaceMatchingSample), InterfaceMatchingSample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(MultipleCommentsTest), MultipleCommentsTest.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(NonGenericCollectionsSample), NonGenericCollectionsSample.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(GenericCollectionsSample), GenericCollectionsSample.GetSampleInstance()));

            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(SerializationOptionsSample), SerializationOptionsSample.GetSampleInstance()));

            //lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(IssuesSample), IssuesSample.GetSampleInstance()));

            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(BookClassTesgingSerializeAsValue), BookClassTesgingSerializeAsValue.GetSampleInstance()));
            lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(CustomSerializationTests), CustomSerializationTests.GetSampleInstance()));

            //lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(FreeSample), FreeSample.GetSampleInstance()));
            //lstSampleClasses.Items.Add(new ClassInfoListItem(typeof(CultureSample), CultureSample.GetSampleInstance()));
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            OnSerialize(false);
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            OnDeserialize(false);
        }

        private void lstSampleClasses_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnSerialize(false);
        }

        private void btnSerializeToFile_Click(object sender, EventArgs e)
        {
            OnSerialize(true);
        }

        private void btnDeserializeFromFile_Click(object sender, EventArgs e)
        {
            OnDeserialize(true);
        }

        private void OnDeserialize(bool openFromFile)
        {
            rtbParsingErrors.Text = "";
            object selItem = lstSampleClasses.SelectedItem;
            if (selItem == null || !(selItem is ClassInfoListItem))
                return;

            string fileName = null;
            if (openFromFile)
            {
                if (DialogResult.OK != openFileDialog1.ShowDialog())
                    return;
                fileName = openFileDialog1.FileName;
            }

            ClassInfoListItem info = selItem as ClassInfoListItem;
            YAXExceptionTypes defaultExType = GetSelectedDefaultExceptionType();
            YAXExceptionHandlingPolicies exPolicy = GetSelectedExceptionHandlingPolicy();
            YAXSerializationOptions serOption = GetSelectedSerializationOption();

            try
            {
                object deserializedObject = null;
                YAXSerializer serializer = new YAXSerializer(info.ClassType, exPolicy, defaultExType, serOption);

                if (openFromFile)
                    deserializedObject = serializer.DeserializeFromFile(fileName);
                else
                    deserializedObject = serializer.Deserialize(rtbXMLOutput.Text);

                rtbParsingErrors.Text = serializer.ParsingErrors.ToString();

                if (deserializedObject != null)
                {
                    rtbDeserializeOutput.Text = deserializedObject.ToString();

                    // TODO: remove later
                    //MessageBox.Show(serializer.Serialize(deserializedObject));

                    if (deserializedObject is List<string>)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in deserializedObject as List<string>)
                        {
                            sb.AppendLine(item.ToString());
                        }
                        MessageBox.Show(sb.ToString());
                    }
                }
                else
                    rtbDeserializeOutput.Text = "The deserialized object is null";
            }
            catch (YAXException ex)
            {
                rtbDeserializeOutput.Text = "";
                MessageBox.Show("YAXException handled:\r\n\r\n" + ex.ToString());
            }
            catch (Exception ex)
            {
                rtbDeserializeOutput.Text = "";
                MessageBox.Show("Other Exception handled:\r\n\r\n" + ex.ToString());
            }
        }

        private void OnSerialize(bool saveToFile)
        {
            object selItem = lstSampleClasses.SelectedItem;
            if (selItem == null || !(selItem is ClassInfoListItem))
                return;

            string fileName = null;
            if (saveToFile)
            {
                if (DialogResult.OK != saveFileDialog1.ShowDialog()) 
                    return;
                fileName = saveFileDialog1.FileName;
            }

            ClassInfoListItem info = selItem as ClassInfoListItem;
            YAXExceptionTypes defaultExType = GetSelectedDefaultExceptionType();
            YAXExceptionHandlingPolicies exPolicy = GetSelectedExceptionHandlingPolicy();
            YAXSerializationOptions serOption = GetSelectedSerializationOption();

            try
            {
                YAXSerializer serializer = new YAXSerializer(info.ClassType, exPolicy, defaultExType, serOption);

                if (saveToFile)
                    serializer.SerializeToFile(info.SampleObject, fileName);
                else
                    rtbXMLOutput.Text = serializer.Serialize(info.SampleObject);
                rtbParsingErrors.Text = serializer.ParsingErrors.ToString();
            }
            catch (YAXException ex)
            {
                MessageBox.Show("YAXException handled:\r\n\r\n" + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Other Exception handled:\r\n\r\n" + ex.ToString());
            }
        }
    }
}
