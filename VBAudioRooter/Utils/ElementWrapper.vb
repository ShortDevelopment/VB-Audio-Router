Imports System.Reflection

Namespace Utils

    Public Class ElementWrapper(Of T As FrameworkElement)

#Region "Constructor"
        Public ReadOnly Property BaseControl As T

        Public Sub New(control As T)
            Me.BaseControl = control
        End Sub
#End Region

        Public Sub Initialize()
            Dim fields = Me.GetType().GetFields(BindingFlags.NonPublic Or BindingFlags.CreateInstance Or BindingFlags.Instance)
            Dim filteredFields = fields.AsParallel().Where(Function(x) x.GetCustomAttributes(False).Select(Function(attr) attr.GetType()).Contains(GetType(ElementChildReferenceAttribute))).ToArray()

            For Each field As FieldInfo In filteredFields
                '' WithEvents fields are translated to properties with the same name
                '' The attribute remains on the backing field with the prefix "_"
                ' Calculate the real property name and therefor the name of the element
                Dim propName As String = field.Name.Replace("_", "")
                ' Find property
                Dim [property] As PropertyInfo = Me.GetType().GetProperty(propName, BindingFlags.CreateInstance Or BindingFlags.Instance Or BindingFlags.NonPublic)
                ' Find element in VisualTree (by name)
                Dim obj As FrameworkElement = BaseControl.FindNameRecursive(propName)
                ' Set value of property and let the (compiler generated) property update all the events
                [property].SetValue(Me, obj)
            Next
        End Sub
    End Class

    <AttributeUsage(AttributeTargets.Field)>
    Friend Class ElementChildReferenceAttribute
        Inherits Attribute
    End Class

End Namespace