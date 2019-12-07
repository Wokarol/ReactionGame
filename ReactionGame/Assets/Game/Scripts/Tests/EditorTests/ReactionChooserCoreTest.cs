using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Wokarol.GameplayCores;

using static TestUtils;

public class ReactionChooserCoreTest
{
    public static class Events
    {
        public const string CoreCreated = "Core Created";
        public const string NewTable = "New Table";
        public const string GameStarted = "Game Started";
    }

    private List<string> eventTimeline;
    private List<Dummy> all;
    private ReactionChooserCore<Dummy> core;

    [SetUp]
    public void Setup()
    {
        eventTimeline = new List<string>();
        all = Dummy.GetList(5);

        // Creating test model
        core = new ReactionChooserCore<Dummy>(all);
        eventTimeline.Add(Events.CoreCreated);
    }

    [Test]
    public void _01_Table_Is_Created_After_Game_Starts()
    {
        core.OnNewTable += (m, ns) => {
            Assert.That(m, Is.Not.Null, "Returned model is equal to null");
            Assert.That(ns, Is.Not.Null, "Candidates are null");
            Assert.That(ns.Count, Is.EqualTo(3), "Incorrect number of candidates");
            eventTimeline.Add(Events.NewTable);
        };

        // Call
        eventTimeline.Add(Events.GameStarted);
        core.NewTable(3);

        // Expectation
        List<string> expected = new List<string>() {
            Events.CoreCreated,
            Events.GameStarted,
            Events.NewTable
        };
        Assert.That(eventTimeline, Is.EqualTo(expected), MessageLists("Event Timeline Missmatch", expected, eventTimeline));
    }

    [Test]
    public void _02_NewTable_Candidates_Have_One_Model()
    {
        Dummy model = null;
        List<Dummy> candidates = null;

        core.OnNewTable += (m, ns) => {
            model = m;
            candidates = ns;
        };

        core.NewTable(3);

        Assert.That(model, Is.Not.Null, "Model wasn't set");
        Assert.That(candidates, Is.Not.Null, "Candidates weren't set");
        Assert.That(candidates.Contains(model), "Candidates don't contain correct answer");
        Assert.That(candidates.Where(d => d == model).Count(), Is.EqualTo(1), "There is more than one correct answer in candidates");
    }

    [Test]
    public void _03_Incorect_Answer_Returns_False()
    {
        Dummy model = null;
        List<Dummy> candidates = null;

        core.OnNewTable += (m, ns) => {
            model = m;
            candidates = ns;
        };

        core.NewTable(3);

        Dummy answer = candidates[0];
        if (answer == model)
            answer = candidates[1];

        Assert.That(core.Answer(answer), Is.False, "Returned value isn't correct");
    }

    [Test]
    public void _04_Corect_Answer_Returns_True()
    {
        Dummy model = null;
        List<Dummy> candidates = null;

        core.OnNewTable += (m, ns) => {
            model = m;
            candidates = ns;
        };

        core.NewTable(3);

        Dummy answer = model;

        Assert.That(core.Answer(answer), Is.True, "Returned value isn't correct");
    }
}