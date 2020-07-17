Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports TechTalk.SpecFlow

Namespace $rootnamespace$

    ' For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    <Binding()> _
    Public NotInheritable Class $safeitemname$

        Private ReadOnly ScenarioContext _scenarioContext;

        Public Sub New(scenarioContext As ScenarioContext)
            _scenarioContext = scenarioContext
        End Sub

        <Given("I have entered (.*) into the calculator")> _
        Public Sub GivenIHaveEnteredSomethingIntoTheCalculator(ByVal number As Integer)
            'TODO: implement arrange (precondition) logic
            ' For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata 
            ' To use the multiline text or the table argument of the scenario,
            ' additional string/Table parameters can be defined on the step definition
            ' method. 

            _scenarioContext.Pending()
        End Sub

        <[When]("I press add")> _
        Public Sub WhenIPressAdd()
            'TODO: implement act (action) logic

            _scenarioContext.Pending()
        End Sub

        <[Then]("the result should be (.*) on the screen")> _
        Public Sub ThenTheResultShouldBe(ByVal result As Integer)
            'TODO: implement assert (verification) logic

            _scenarioContext.Pending()
        End Sub


    End Class

End Namespace
