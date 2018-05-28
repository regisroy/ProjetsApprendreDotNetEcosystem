﻿using TechTalk.SpecFlow;

namespace SpecFlowProject.Specs
{
	[Binding]
	public class CalculatorSteps
	{
		[Given(@"I have entered (.*) into the calculator")]
		public void GivenIHaveEnteredIntoTheCalculator(int p0)
		{
			ScenarioContext.Current.Pending();
		}


		[Given(@"I have also entered (.*) into the calculator")]
		public void GivenIHaveAlsoEnteredIntoTheCalculator(int p0)
		{
			ScenarioContext.Current.Pending();
		}

		[When(@"I press add")]
		public void WhenIPressAdd()
		{
			ScenarioContext.Current.Pending();
		}

		[Then(@"The result should be (.*) on the screen")]
		public void ThenTheResultShouldBeOnTheScreen(int p0)
		{
			ScenarioContext.Current.Pending();
		}
	}
}