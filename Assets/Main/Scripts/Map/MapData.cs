using Kartograph.Entities;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    private LevelGenerator3D levelGenerator;
    public SectionData[] sectionsData;
    public Section[] section;
    public List<Transform> allWayPoints;
    public MapData(LevelGenerator3D levelGenerator) 
    {
        this.levelGenerator = levelGenerator;
        section = levelGenerator.transform.GetComponentsInChildren<Section>();
        sectionsData = new SectionData[section.Length];
        Debug.Log(levelGenerator.transform.childCount);
        for (int i = 0; i < sectionsData.Length; i++)
        {
            sectionsData[i] = new SectionData(section[i], i);
        }
        allWayPoints = new List<Transform>();
        foreach (var section in sectionsData)
        {
            foreach (var point in section.points)
            {
                allWayPoints.Add(point);
            }
        }
    }

    public class SectionData
    {
        public WayPoints wayPoints { get; private set; }
        public Transform[] points { get; private set; }


        public SectionData(Section section,int i)
        {
            if (section == null)
                Debug.Log("sa " + i);
            wayPoints = section.GetComponentInChildren<WayPoints>();
            if (wayPoints != null)
            {
                points = wayPoints.GetComponentsInChildren<Transform>();
            }
        }
    }
}
