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

    [Test]
    public void _01_Table_Is_Created_After_Game_Starts()
    {
        List<string> eventTimeline = new List<string>();
        List<Dummy> all = Dummy.GetList(5);

        // Creating test model
        var core = new ReactionChooserCore<Dummy>(all);
        eventTimeline.Add(Events.CoreCreated);

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
        List<Dummy> all = Dummy.GetList(5);
        var core = new ReactionChooserCore<Dummy>(all);

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

}