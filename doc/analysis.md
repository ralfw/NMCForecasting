# Monte Carlo Forecasting Analysis
Forecasting is understood as prediction based on historical data. Example:

> Predict the next role of a dice based on previous roles, e.g. [4,5,6,2,1,5,3,3,4,1,4,1,2,2,4,6,3,5,1].

Based on the historical data a probability distribution can be calculated:

| Roled | Frequency | Probability	 | Percentile |
|:---:|:---:|:---:|:---:|
| 1	 | 4 | 0,21 | 21%
| 2	 | 3 | 0,16 | 37%
| 3	 | 3 | 0,16 | 53%
| 4	 | 4 | 0,21 | 74%
| 5	 | 3 | 0,16 | 89%
| 6	 | 2 | 0,11 | 100%

According to this distribution `1` and `4` are the most likely next roles (both with a probability of 0,21).

But also: The probability of the next role being `3` *or less* is 0,53.

Of course with dice historical data is not needed. The probabilities are obvious from the beginning: for every face of the dice it's 1/6=0,1667. At least if the dice is fair.

If it's not known if a dice is fair or with any other events with a priori unknown probabilities historical data can be used to calculate (approximate) probabilities.

Predicting a single next event can be done using the the distribution like above.

**Terminology**

* **Event**: Something that can happen, e.g. when roling a dice there are six possible events: `1`, `2`, `3`, `4`, `5`, `6`.
* **Type** of event: *face of dice* or *side of coin*
* **Observation**: an event that happened, e.g. a dice was roled and shows a `2`.
* **Historical data**: a log of observations
* **Probability**: Likelihood that an event will be observed. Calculated by dividing the number of observations (e.g. 2) of a certain event (e.g. `6`) by the total number of observations (e.g. 19), e.g. 2/19=0,11.
* **Percentile**: Percentage of observations up to and including a certain event (e.g. `3`) in an ordered list of events (e.g. `[1,2,3,4,5,6]`), e.g. (4+3+3)/19x100=53%. The percentile can be interpreted as the probability of an event being one from the set of events, e.g. the probability of roling a `1` *or* `2` *or* `3` (equals: `3` *or less*) is 0,53. Pre-requisite: events are independent of each other.

## Forecasting combinations of events
"What's the probability of the next two roles of a dice being first `2` and then `3`?" or "What's the risk of the the sum of the next two roles of a dice being 6 or lower?"

Such questions cannot be answered by just looking at a probability distribution, althoug. The answer needs to be either calculated - or can be simulated using the Monte Carlo method.

Combinations of events define a new type of event with its own events, e.g.

* Next two roles being first `2` and then `3`: `[true, false]`
* Sum of the next two roles: `[2..12]`






combinations of event from different types


