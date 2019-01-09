[<TechTalk.SpecFlow.Binding>]
module $safeitemname$

open TechTalk.SpecFlow

// For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

let [<Given>] ``I have entered (.*) into the calculator``(number:int) = 
    ScenarioContext.StepIsPending() //TODO: implement arrange (precondition) logic

let [<When>] ``I press add``() = 
    ScenarioContext.StepIsPending() //TODO: implement act (action) logic

let [<Then>] ``the result should be (.*) on the screen``(result:int) = 
    ScenarioContext.StepIsPending() //TODO: implement assert (verification) logic
