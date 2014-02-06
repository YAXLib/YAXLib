Imports System.Reflection
Imports System.IO
Imports RnD.BusinessLayer.EF.Factory
Imports RnD.BusinessLayer.Interfaces.Model
Imports RnD.BusinessLayer.Interfaces.Enums
Imports RnD.BusinessLayer.EF
Imports NUnit.Framework
Imports RnD.BusinessLayer.EF.Sample
Imports RnD.BusinessLayer.Interfaces.Sample
Imports System.Xml.Serialization
Imports System.Xml
Imports YAXLib

Public Class InterpreterTest
    Private Shared Function GetSampleInstance() As RnD.BusinessLayer.EF.Sample.Sample
        Return New RnD.BusinessLayer.EF.Sample.Sample(New Session("MES-devpc7_SQLAuth", DatabaseAccessMode.ConfigurationalObjectsUserAccess))
    End Function

    <Test()>
    Public Sub CreateSampleTestWithoutFactory()
        Dim lSession = New Session("MES-devpc7_SQLAuth", DatabaseAccessMode.ConfigurationalObjectsUserAccess, Nothing, New TaskDetails() With {.Task = "OnSampleCreate", .TaskType = "sccreate"})
        Dim lObjectFactory = New ObjectFactory(lSession)
        
        Dim lSampleInstance = GetSampleInstance()
        Dim lSample = lSampleInstance.GetFull(22)
        Dim lRequest = lObjectFactory.Request.GetFull(1)


        Dim lFileContent = File.ReadAllText("UnilinkExample.xml")

        Dim lOperation1 = New Operation
        lOperation1.Name = "create_sc"
        lOperation1.Value = "Y"
        Dim lArguments = New List(Of Argument)
        Dim lArgument = New Argument
        lArguments.Add(lArgument)
        lOperation1.Arguments = lArguments


        Dim lTransactions = New List(Of Transaction)
        Dim lTransaction1 = New Transaction
        lTransaction1.Operations = New List(Of Operation)()
        lTransaction1.Operations.Add(lOperation1)
        lTransactions.Add(lTransaction1)

        Dim lUnilinkFile = New RnDSuiteFile
        lUnilinkFile.DataSection = New Data()
        lUnilinkFile.DataSection.Samples = New List(Of SampleFullDTO)()
        lUnilinkFile.DataSection.Samples.Add(lSample)
        lUnilinkFile.DataSection.Requests = New List(Of RequestFullDTO)()
        lUnilinkFile.DataSection.Requests.Add(lRequest)
        lUnilinkFile.OperationsSection = New OperationSection()
        lUnilinkFile.OperationsSection.Transactions = lTransactions

        Dim lSerializer = New YAXSerializer(GetType(RnDSuiteFile), True)
        Dim lSerializedFile = lSerializer.Serialize(lUnilinkFile)

        Dim lAssemblyToSearch = GetType(Request.Request).Assembly
        Dim lTypes = From lType In lAssemblyToSearch.GetTypes()
                     Where Attribute.IsDefined(lType, GetType(ManagerForAttribute))
        Dim lManagerType As Type = Nothing

        lManagerType = GetManagerType(lTypes, lManagerType)

        Dim lDeserializedFile As RnDSuiteFile = lSerializer.Deserialize(lSerializedFile)
        For Each lTransaction As Transaction In lDeserializedFile.OperationsSection.Transactions
            'OPEN TRANSACTION HANDLING
            For Each lOperation As Operation In lTransaction.Operations
                Dim lUnilinkFunction As UnilinkFunction = GetUnilinkFunction(lOperation, lManagerType)

                'Get id of the object which is context
                InvokeOperation(lUnilinkFunction, lObjectFactory, lDeserializedFile.DataSection.Samples.FirstOrDefault())
            Next
            lObjectFactory.SaveChanges("From unillink!")
            'End transaction handling
        Next


      
      
    End Sub
  

    Private Sub InvokeOperation(ByVal aUnilinkFunction As UnilinkFunction, ByVal aObjectFactory As ObjectFactory, ByVal sampleFullDTO As SampleFullDTO)
        Dim lPropertyManager = (From lProperty In aObjectFactory.GetType().GetProperties()
                               Where lProperty.PropertyType = GetType(ISample)).FirstOrDefault()

        If lPropertyManager IsNot Nothing Then
            Dim lPropertyInstance = lPropertyManager.GetValue(aObjectFactory, Nothing)
            Dim lParameters = GetAPIArguments(aUnilinkFunction.Arguments, sampleFullDTO)
            Dim lMethodResult = aUnilinkFunction.Method.Invoke(lPropertyInstance, lParameters.ToArray())
        End If
    End Sub

    Private Function GetAPIArguments(ByVal aArguments As IEnumerable, ByVal aSampleFullDTO As SampleFullDTO) As List(Of Object)
        Dim lResult As New List(Of Object)


        For Each aArgument As String In aArguments
            Dim lProperty = aSampleFullDTO.GetType().GetProperty(aArgument)
            Dim lArgumentValue = lProperty.GetValue(aSampleFullDTO, Nothing)
            lResult.Add(lArgumentValue)
        Next

        lResult.Add("sccreate")
        lResult.Add("OnSampleCreate")
        lResult.Add(Nothing)

        Return lResult
    End Function

    Private Function GetUnilinkFunction(ByVal lOperation As Operation, ByVal lManagerType As Type) As UnilinkFunction
        Dim lUnilinkFunction As UnilinkFunction

        Dim lUnilinkMethodInfo As MethodInfo = Nothing
        Dim lUnilinkFunctionArguments As String() = Nothing

        Dim lUnilinkFunctions = From lMethod In lManagerType.GetMethods(BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Public)
                Where Attribute.IsDefined(lMethod, GetType(UnilinkFunctionAttribute))

        For Each lFunction In lUnilinkFunctions
            Dim lAttribute = lFunction.GetCustomAttributes(False).OfType(Of UnilinkFunctionAttribute)().FirstOrDefault()
            If lAttribute.Name = lOperation.Name Then
                lUnilinkMethodInfo = lFunction
                lUnilinkFunctionArguments = lAttribute.ArgumentNames
            End If
        Next

        lUnilinkFunction = New UnilinkFunction()
        lUnilinkFunction.Method = lUnilinkMethodInfo
        lUnilinkFunction.Arguments = lUnilinkFunctionArguments

        Return lUnilinkFunction
    End Function

    Private Function GetManagerType(ByVal lTypes As IEnumerable(Of Type), ByVal lManagerType As Type) As Type

        For Each lType As Type In lTypes
            Dim lAttribute = lType.GetCustomAttributes(False).OfType(Of ManagerForAttribute)().FirstOrDefault()

            If lAttribute Is Nothing Then
                Continue For
            End If

            If lAttribute.ManagedType = GetType(SampleFullDTO) Then
                lManagerType = lType
            End If
        Next

        If lManagerType Is Nothing Then
            Throw New Exception("Manager type not found!")
        End If
        Return lManagerType
    End Function
End Class

Public Class UnilinkFunction

    Public Property Method As MethodInfo
    Public Property Arguments As String()

End Class
