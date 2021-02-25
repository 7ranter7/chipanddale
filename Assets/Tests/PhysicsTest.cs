using System.Collections;
using System.Collections.Generic;
using ChipNDale;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ray = ChipNDale.Ray;

namespace Tests
{
    public class PhysicsTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void PhysicsCircleCircleCollision()
        {
            Circle circle1 = new Circle(new Vector2(0, 0), 1);
            Circle circle2 = new Circle(new Vector2(3, 0), 1);
            bool col = circle1.PointCollision(new Vector2(5, 0), circle2, out var collision);
            
            Debug.Log($"{collision.Normal} {collision.CorrectPointAfterCollision} {collision.Reflection}");
            Assert.IsTrue(col);
        }

        
        
        [Test]
        public void PhysicsRayCastCircle()
        {
            Circle circle = new Circle(new Vector2(-4, 0), 1);
            var raycast = circle.RayCast(new Ray(Vector2.down, Vector2.left), out var nearestPoint,
                out var farthestPoint, 0);
            Assert.IsTrue(raycast);
            
            Debug.Log($"{nearestPoint} {farthestPoint}");
            
            raycast = circle.RayCast(new Ray(Vector2.up/2, Vector2.right), out nearestPoint,
                out farthestPoint, 0.5f);
            Assert.IsFalse(raycast);
        }
        
        [Test]
        public void PhysicsInCircle()
        {
            Circle circle = new Circle(new Vector2(-4, 0), 1);
            var inCollider = circle.InCollider(new Vector2(0,0),0.5f);
            Assert.IsFalse(inCollider);
            
            inCollider = circle.InCollider(new Vector2(0,0),3);
            Assert.IsTrue(inCollider);
            
            inCollider = circle.InCollider(new Vector2(-1.5f,0),2);
            Assert.IsTrue(inCollider);
        }
        
        [Test]
        public void PhysicsClosestPointCircle()
        {
            Circle circle = new Circle(new Vector2(-4, 0), 1);
            //var inCollider = circle.InCollider(new Vector2(0,0),0.5f);
            //Assert.IsFalse(inCollider);
            
            //inCollider = circle.InCollider(new Vector2(0,0),3);
            //Assert.IsFalse(inCollider);
            
            var point = circle.ClosestPoint(new Vector2(0,0),out var straight,1);
            Debug.Log($"{straight.Distance} {straight.Normal} {point}");
            Assert.IsTrue(point==new Vector2(-2,0));
        }
    }
}
