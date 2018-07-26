// SCIENCE BIRDS: A clone version of the Angry Birds game used for 
// research purposes
// 
// Copyright (C) 2016 - Lucas N. Ferreira - lucasnfe@gmail.com
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>
//

﻿using UnityEngine;
using System.Collections;

public class ABPig : ABCharacter {

	public override void Die(bool withEffect = true)
	{
		ScoreHud.Instance.SpawnScorePoint(50, transform.position);
		ABGameWorld.Instance.KillPig(this);

		base.Die(withEffect);
	}

    //public override void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.name == "Ground" && collision.gameObject.tag != "test")
    //    {
    //        ABLevel addGround = LevelList.Instance.GetCurrentLevel();
    //        foreach (ContactPoint2D groundPoint in collision.contacts)
    //        {
    //            if (!addGround.grounds.Contains(groundPoint.point.x) && groundPoint.point.x <= 5)
    //            {
    //                addGround.grounds.Add(groundPoint.point.x);
    //            }

    //        }
    //    }
    //}
}
