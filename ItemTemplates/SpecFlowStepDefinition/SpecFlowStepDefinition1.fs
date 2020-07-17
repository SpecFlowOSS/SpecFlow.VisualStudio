[<TechTalk.SpecFlow.Binding>]
module $safeitemname$

open TechTalk.SpecFlow

// For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

let [<Given>] ``the first number is (.*)``(number:int) = 
    ScenarioContext.StepIsPending() //TODO: implement arrange (precondition) logic

let [<Given>] ``the second number is (.*)``(number:int) = 
    ScenarioContext.StepIsPending() //TODO: implement arrange (precondition) logic

let [<When>] ``the two numbers are added``() = 
    ScenarioContext.StepIsPending() //TODO: implement act (action) logic

let [<Then>] ``the result should be (.*)``(result:int) = 
    ScenarioContext.StepIsPending() //TODO: implement assert (verification) logic
