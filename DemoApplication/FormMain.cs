// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using YAXLib;
using YAXLibTests.SampleClasses;
using System.Linq;
using YAXLib.Enums;
using YAXLib.Exceptions;
using YAXLib.Options;
using YAXLibTests;

namespace DemoApplication;

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
        if (comboErrorType.Items.Count > 0)
            comboErrorType.Text = YAXExceptionTypes.Error.ToString();
        if (comboOptions.Items.Count > 0)
            comboOptions.Text = YAXSerializationOptions.SerializeNullObjects.ToString();
    }

    private YAXExceptionTypes GetSelectedDefaultExceptionType()
    {
        return (YAXExceptionTypes) Enum.Parse(typeof(YAXExceptionTypes), comboErrorType.Text);
    }

    private YAXExceptionHandlingPolicies GetSelectedExceptionHandlingPolicy()
    {
        return (YAXExceptionHandlingPolicies) Enum.Parse(typeof(YAXExceptionHandlingPolicies), comboPolicy.Text);
    }

    private YAXSerializationOptions GetSelectedSerializationOption()
    {
        return (YAXSerializationOptions) Enum.Parse(typeof(YAXSerializationOptions), comboOptions.Text);
    }

    private void InitListOfClasses()
    {
        var autoLoadTypes = typeof(Book).Assembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(ShowInDemoApplicationAttribute), false).Length == 0)
            .Select(t => new {
                Type = t,
                Attr = t.GetCustomAttributes(typeof(ShowInDemoApplicationAttribute), false)
                        .FirstOrDefault()
                    as ShowInDemoApplicationAttribute
            })
            .Select(pair =>
            {
                var sortKey = pair.Type.Name;
                var attr = pair.Attr;
                if (attr != null && !string.IsNullOrEmpty(attr.SortKey))
                    sortKey = attr.SortKey;
                var sampleInstanceMethod = "GetSampleInstance";
                if (attr != null && !string.IsNullOrEmpty(attr.GetSampleInstanceMethodName))
                    sampleInstanceMethod = attr.GetSampleInstanceMethodName;

                return
                    new { Type = pair.Type, SortKey = sortKey, SampleInstanceMethod = sampleInstanceMethod };
            }).OrderBy(pair => pair.SortKey);

        var sb = new StringBuilder();
        foreach (var tuple in autoLoadTypes)
        {
            try
            {
                var type = tuple.Type;
                var method = type.GetMethod(tuple.SampleInstanceMethod, Type.EmptyTypes);
                var instance = method?.Invoke(null, null);
                lstSampleClasses.Items.Add(new ClassInfoListItem(type, instance!));
            }
            catch
            {
                sb.AppendLine(tuple.Type.FullName);
            }
        }

        if (sb.Length > 0)
        {
            MessageBox.Show(
                "Please provide a parameterless public static method called \"GetSampleInstance\" for the following classes:"
                + Environment.NewLine + sb);
        }
    }

    private void BtnSerialize_Click(object sender, EventArgs e)
    {
        OnSerialize(false);
    }

    private void BtnDeserialize_Click(object sender, EventArgs e)
    {
        OnDeserialize(false);
    }

    private void LstSampleClasses_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        OnSerialize(false);
    }

    private void BtnSerializeToFile_Click(object sender, EventArgs e)
    {
        OnSerialize(true);
    }

    private void BtnDeserializeFromFile_Click(object sender, EventArgs e)
    {
        OnDeserialize(true);
    }

    private void OnDeserialize(bool openFromFile)
    {
        rtbParsingErrors.Text = "";
        var selItem = lstSampleClasses.SelectedItem;
        if (selItem is not ClassInfoListItem info)
            return;

        var fileName = string.Empty;
        if (openFromFile)
        {
            if (DialogResult.OK != openFileDialog1.ShowDialog())
                return;
            fileName = openFileDialog1.FileName;
        }

        var defaultExType = GetSelectedDefaultExceptionType();
        var exPolicy = GetSelectedExceptionHandlingPolicy();
        var serOption = GetSelectedSerializationOption();

        try
        {
            object? deserializedObject;
            var serializer = new YAXSerializer(info.ClassType, new SerializerOptions {
                ExceptionHandlingPolicies = exPolicy,
                ExceptionBehavior = defaultExType,
                SerializationOptions = serOption, MaxRecursion = Convert.ToInt32(numMaxRecursion.Value)
            });

            if (openFromFile)
                deserializedObject = serializer.DeserializeFromFile(fileName);
            else
                deserializedObject = serializer.Deserialize(rtbXMLOutput.Text);

            rtbParsingErrors.Text = serializer.ParsingErrors.ToString();

            if (deserializedObject != null)
            {
                rtbDeserializeOutput.Text = deserializedObject.ToString();

                if (deserializedObject is List<string> list)
                {
                    var sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        sb.AppendLine(item);
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
        var selItem = lstSampleClasses.SelectedItem;
        if (selItem is not ClassInfoListItem info)
            return;

        var fileName = string.Empty;
        if (saveToFile)
        {
            if (DialogResult.OK != saveFileDialog1.ShowDialog())
                return;
            fileName = saveFileDialog1.FileName;
        }

        var defaultExType = GetSelectedDefaultExceptionType();
        var exPolicy = GetSelectedExceptionHandlingPolicy();
        var serOption = GetSelectedSerializationOption();

        try
        {
            var serializer = new YAXSerializer(info.ClassType, new SerializerOptions {
                ExceptionHandlingPolicies = exPolicy,
                ExceptionBehavior = defaultExType,
                SerializationOptions = serOption, MaxRecursion = Convert.ToInt32(numMaxRecursion.Value)
            });

            if (saveToFile)
                serializer.SerializeToFile(info.SampleObject, fileName);
            else
                rtbXMLOutput.Text = serializer.Serialize(info.SampleObject);
            rtbParsingErrors.Text = serializer.ParsingErrors.ToString();
        }
        catch (YAXException ex)
        {
            MessageBox.Show("YAXException handled:\r\n\r\n" + ex);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Other Exception handled:\r\n\r\n" + ex);
        }
    }
}
