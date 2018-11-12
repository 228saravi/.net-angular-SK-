using System;
using System.Collections;
using System.Collections.Generic;
using SK.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace SK.Domain
{
  public class SkillsMatrix
  {
    public class Specialization
    {
      public string Id { get; set; }
      public string Name { get; set; }
      public int Rank { get; set; }
    }

    public class SkillsGroup
    {
      public string Name { get; set; }
      public Skill[] Skills { get; set; }
    }

    public class Skill
    {
      public string Id { get; set; }
      public string Name { get; set; }
      public int Rank { get; set; }
      public string GroupName { get; set; }
    }

    public class Speciality
    {
      public string Id { get; set; }
      public string Name { get; set; }
      
      public Specialization[] Specializations { get; set; }
      public SkillsGroup[] SkillsGroups { get; set; }
      public Skill[] Skills { get; set; }
    }

    public Speciality[] Specialities { get; set; }
  }

  public class SkillsMatrixDirectory
  {
    public async Task<SkillsMatrix> Get(DatabaseContext database)
    {
      return new SkillsMatrix
      {
        Specialities = await database.Specialities.Include(s => s.Skills).OrderBy(skill => skill.Rank).ThenBy(skill => skill.Name).Select(s => new SkillsMatrix.Speciality
        {
          Id = s.Id,
          Name = s.Name,
          Specializations = s.Specializations.OrderBy(x => x.Rank).Select(specialization => new SkillsMatrix.Specialization
          {
            Id = specialization.Id,
            Name = specialization.Name,
            Rank = specialization.Rank,
          }).ToArray(),
          Skills = s.Skills.OrderBy(skill => skill.Rank).Select(sp => new SkillsMatrix.Skill
          {
            Id = sp.Id,
            Name = sp.Name,
            Rank =  sp.Rank,
            GroupName = sp.GroupName,
          }).ToArray(),
          SkillsGroups = s.Skills.OrderBy(skill => skill.Rank).Select(sp => new SkillsMatrix.Skill
          {
            Id = sp.Id,
            Name = sp.Name,
            Rank = sp.Rank,
            GroupName = sp.GroupName,
          }).GroupBy(sk => sk.GroupName).Select(g => new SkillsMatrix.SkillsGroup { Name = g.Key, Skills = g.ToArray() }).ToArray()
        }).ToArrayAsync()
      };
    }
  }
}