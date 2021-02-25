using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace ChipNDale
{
    public struct Straight
    {
        public Vector2 Normal { get; }
        public float Distance { get; }

        public Straight(Vector2 normal, float distance)
        {
            Normal = normal;
            Distance = distance;
            if (Normal.magnitude == 0) throw new Exception("Normal magnitude equal zero.");
        }

        public Straight(Vector2 a, Vector2 b)
        {
            Normal = new Vector2(a.y - b.y, b.x - a.x);
            Distance = a.x * b.y - b.x * a.y;
            var magnitude = Normal.magnitude;
            if (magnitude == 0) throw new Exception("Normal magnitude equal zero.");
            Normal = Normal / magnitude;
            Distance = Distance / magnitude;
        }

        public float DistanceToPoint(Vector2 point)
        {
            var dis = Normal.x * point.x + Normal.y * point.y + Distance;
            //if (dis >= 0) dis -= radius;
            return dis;
        }

        public Vector2 Projection(Vector2 point)
        {
            var distance = DistanceToPoint(point);
            return point + Normal * distance;
        }

        public bool BehindOrOnStraight(Vector2 point, float radius = 0)
        {
            var distance = DistanceToPoint(point);
            if (distance <= -radius && distance <= radius)
                return true;
            return false;
        }

        public bool BehindStraight(Vector2 point, float radius = 0)
        {
            var distance = DistanceToPoint(point);
            if (distance < -radius)
                return true;
            return false;
        }

        public bool InFrontOrOnStraight(Vector2 point, float radius = 0)
        {
            var distance = DistanceToPoint(point);
            if (distance >= -radius && distance <= radius)
                return true;
            return false;
        }

        public bool InFrontStraight(Vector2 point, float radius = 0)
        {
            var distance = DistanceToPoint(point);
            if (distance > radius)
                return true;
            return false;
        }

        public bool RayIntersect(Ray ray, out Vector2 intersection, float radius = 0)
        {
            var direction = ray.Direction.normalized;
            var divider = Normal.x * direction.x + Normal.y * direction.y;
            var distance = Distance + Normal.x * ray.Position.x + Normal.y * ray.Position.y;
            if (divider < 0)
            {
                distance /= -divider;
                if (radius != 0)
                {
                    var projection = Projection(ray.Position);
                    var inProjection = projection - ray.Position;
                    var cos = Vector2.Dot(inProjection.normalized, direction.normalized);
                    var diff = radius / cos;
                    distance += diff;
                }

                intersection = ray.Position + direction * distance;
                return true;
            }

            intersection = Vector2.zero;
            return false;
        }

        public bool RayPushFrom(Ray ray, out Vector2 intersection, float radius = 0)
        {
            var direction = ray.Direction.normalized;
            var divider = Normal.x * direction.x + Normal.y * direction.y;
            var distance = Distance + Normal.x * ray.Position.x + Normal.y * ray.Position.y;
            if (divider > 0)
            {
                distance /= -divider;
                if (radius != 0)
                {
                    var projection = Projection(ray.Position);
                    var inProjection = projection - ray.Position;
                    var cos = Vector2.Dot(inProjection.normalized, direction.normalized);
                    var diff = radius / cos;
                    distance -= diff;
                }

                intersection = ray.Position + direction * distance;
                return true;
            }

            intersection = Vector2.zero;
            return false;
        }
    }

    public struct Ray
    {
        public Vector2 Position { get; }
        public Vector2 Direction { get; }

        public Ray(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = direction;
        }

        public bool DistanceToPoint(Vector2 point, out float distance)
        {
            var dot = Vector2.Dot((Position - point).normalized, Direction);
            if (dot == 1 || dot == -1)
            {
                distance = dot * (Position - point).magnitude;
                return true;
            }

            distance = 0;
            return false;
        }
    }

    public struct Line
    {
        private Straight Straight;
        private Straight Right;
        private Straight Left;
        public Vector2 Point1 { get; }
        public Vector2 Point2 { get; }

        public Line(Vector2 a, Vector2 b)
        {
            Point1 = a;
            Point2 = b;
            Straight = new Straight(a, b);
            Right = new Straight(a + Straight.Normal, a);
            Left = new Straight(b, b + Straight.Normal);
        }

        public float DistanceToLine(Vector2 point)
        {
            var s = Straight.DistanceToPoint(point);
            var r = Right.DistanceToPoint(point);
            var l = Left.DistanceToPoint(point);
            if (r >= 0 && l >= 0) return s;

            if (l <= 0) return Mathf.Sign(s) * (Point1 - point).magnitude;

            if (r <= 0) return Mathf.Sign(s) * (Point2 - point).magnitude;

            return 0;
        }
    }


    public struct Polygon : ICollider
    {
        public List<Vector2> Points { get; }
        public List<Straight> Sides { get; }
        public float Radius { get; }
        private Vector2 position;

        public Vector2 Position
        {
            get => position;
            set { UpdatePosition(value); }
        }

        private void UpdatePosition(Vector2 newPosition)
        {
            var diff = newPosition - position;
            for (var i = 0; i < Points.Count; i++) Points[i] += diff;
            for (var i = 0; i < Points.Count; i++) Sides[i] = new Straight(Points[i], Points[(i + 1) % Points.Count]);
            position = newPosition;
        }


        public Polygon(Vector2 position, params Vector2[] points)
        {
            if (points.Length < 3) throw new Exception("Polygon is needed to have minimum 3 points.");
            Radius = 0;
            Points = new List<Vector2>(points);
            Sides = new List<Straight>();
            this.position = Vector2.zero;
            foreach (var p in points)
            {
                Sides.Add(new Straight());
            }

            UpdatePosition(position);
        }

        public Polygon(List<Vector2> points)
        {
            if (points.Count < 3) throw new Exception("Polygon is needed to have minimum 3 points.");
            Radius = 0;
            Points = new List<Vector2>(points);
            Sides = new List<Straight>();
            position = Vector2.zero;
            foreach (var p in points)
            {
                position += p;
                Sides.Add(new Straight());
            }

            position /= points.Count;
            UpdatePosition(position);
        }


        public bool PointCollision(Vector2 newPos, ICollider collider, out Collision collision)
        {
            var moveDirection = newPos - Position;
            collision = new Collision();

            bool intersected = false;
            foreach (var point in Points)
            {
                if (collider.InCollider(point))
                {
                    var closestPoint = collider.ClosestPoint(point, out var straight, Radius);
                    collision.Normal = straight.Normal;
                    collision.CorrectPointAfterCollision = Position + closestPoint - point;
                    collision.Reflection = collision.CorrectPointAfterCollision;
                    if (collision.ContactPoints == null)
                        collision.ContactPoints = new List<Vector2>();
                    collision.ContactPoints.Add(closestPoint);
                    collision.OtherCollider = collider;
                    intersected = true;
                }
            }

            if (intersected) return true;

            if (moveDirection.sqrMagnitude == 0)
            {
                foreach (var point in Points)
                {
                    if (collider.InCollider(point))
                    {
                        var closestPoint = collider.ClosestPoint(point, out var straight, Radius);
                        collision.Normal = straight.Normal;
                        collision.CorrectPointAfterCollision = Position + closestPoint - point;
                        collision.Reflection = collision.CorrectPointAfterCollision;
                        if (collision.ContactPoints == null)
                            collision.ContactPoints = new List<Vector2>();
                        collision.ContactPoints.Add(closestPoint);
                        collision.OtherCollider = collider;
                        intersected = true;
                    }
                }

                return intersected;
            }
            else
            {
                foreach (var point in Points)
                {
                    var ray = new Ray(point, moveDirection);
                    if (collider.RayCast(ray, out var nearestPoint, out var farthestPoint, 0))
                    {
                        var oldPositionNearestDiff = point - nearestPoint;
                        if (oldPositionNearestDiff.sqrMagnitude <= moveDirection.sqrMagnitude)
                        {
                            var closestPoint = collider.ClosestPoint(nearestPoint, out var straight, Radius);
                            collision.Normal = straight.Normal;
                            collision.CorrectPointAfterCollision = closestPoint + (Position - point);
                            collision.Reflection = collision.CorrectPointAfterCollision +
                                                   Vector2.Reflect(ray.Direction + (point - nearestPoint),
                                                       straight.Normal);
                            if (collision.ContactPoints == null)
                                collision.ContactPoints = new List<Vector2>();
                            collision.ContactPoints.Add(nearestPoint);
                            collision.OtherCollider = collider;
                        }
                    }
                }

                if (collider.Points != null)
                    foreach (var point in collider.Points)
                    {
                        var ray = new Ray(point, -moveDirection);
                        if (RayCast(ray, out var nearestPoint, out var farthestPoint, 0))
                        {
                            var oldPositionNearestDiff = nearestPoint - point;
                            if (oldPositionNearestDiff.sqrMagnitude <= moveDirection.sqrMagnitude &&
                                (collision.ContactPoints.Count == 0 ||
                                 oldPositionNearestDiff.sqrMagnitude <
                                 (Position - collision.CorrectPointAfterCollision).sqrMagnitude))
                            {
                                var closestPoint =collider.ClosestPoint(nearestPoint, out var straight, Radius);
                                collision.Normal = straight.Normal;
                                collision.CorrectPointAfterCollision =closestPoint + (Position - nearestPoint);
                                collision.Reflection = collision.CorrectPointAfterCollision +
                                                       Vector2.Reflect(ray.Direction + (point-nearestPoint),
                                                           (point-nearestPoint).normalized);
                                if (collision.ContactPoints == null)
                                    collision.ContactPoints = new List<Vector2>();
                                collision.ContactPoints.Add(nearestPoint);
                                collision.OtherCollider = collider;
                            }
                        }
                    }

                if (collision.ContactPoints != null && collision.ContactPoints.Count != 0)
                    return true;
                return false;
            }
        }

        public bool InCollider(Vector2 point, float radius = 0)
        {
            var inside = true;
            foreach (var s in Sides)
                if (s.DistanceToPoint(point) > radius)
                    inside = false;

            return inside;
        }

        public Vector2 ClosestPoint(Vector2 point, out Straight straight, float radius = 0)
        {
            var minDistance = float.PositiveInfinity;
            var closestPoint = Vector2.zero;
            var ray = new Ray(point, Position - point);
            straight = new Straight();

            if (InCollider(point))
            {
                foreach (var s in Sides)
                {
                    if (s.RayPushFrom(new Ray(point, s.Normal), out var intersection, radius))
                    {
                        var distance = (point - intersection).sqrMagnitude;
                        if (InCollider(intersection, radius))
                        {
                            closestPoint = intersection;
                            straight = s;
                            return closestPoint;
                        }
                    }
                }
            }
            else
                foreach (var s in Sides)
                {
                    if (s.RayIntersect(new Ray(point, -s.Normal), out var intersection, radius))
                    {
                        var distance = (point - intersection).sqrMagnitude;
                        if (InCollider(intersection, radius))
                            if (distance < minDistance)
                            {
                                minDistance = Mathf.Abs(distance);
                                closestPoint = intersection;
                                straight = s;
                            }
                    }
                }

            return closestPoint;
        }

        public bool RayCast(Ray ray, out Vector2 nearestPoint, out Vector2 farthestPoint, float radius = 0)
        {
            var intersected = false;
            nearestPoint = farthestPoint = Vector2.zero;
            foreach (var s in Sides)
                if (s.RayIntersect(ray, out var intersection, radius))
                {
                    var onPolygon = true;
                    foreach (var s2 in Sides)
                        if (s2.InFrontStraight(intersection, radius))
                        {
                            onPolygon = false;
                            break;
                        }

                    if (onPolygon)
                    {
                        if (intersected)
                        {
                            var diffNearest = ray.Position - nearestPoint;
                            var diffFarthest = ray.Position - intersection;
                            if (diffFarthest.sqrMagnitude >= diffNearest.sqrMagnitude)
                            {
                                farthestPoint = intersection;
                            }
                            else
                            {
                                farthestPoint = nearestPoint;
                                nearestPoint = intersection;
                            }
                        }
                        else
                        {
                            intersected = true;
                            nearestPoint = intersection;
                        }
                    }
                }

            return intersected;
        }
    }

    public struct Circle : ICollider
    {
        public List<Vector2> Points { get; }
        public List<Straight> Sides { get; }
        public float Radius { get; }
        public Vector2 Position { get; set; }

        public Circle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
            Sides = null;
            Points = null;
        }

        public bool PointCollision(Vector2 newPos, ICollider collider, out Collision collision)
        {
            var moveDirection = newPos - Position;
            collision = new Collision();
            if (moveDirection.sqrMagnitude == 0)
            {
                if (collider.InCollider(Position, Radius))
                {
                    var closestPoint = collider.ClosestPoint(Position, out var straight, Radius);
                    collision.Normal = straight.Normal;
                    collision.CorrectPointAfterCollision = closestPoint;
                    collision.Reflection = closestPoint;
                    collision.ContactPoints = new List<Vector2>();
                    collision.ContactPoints.Add(closestPoint);
                    collision.OtherCollider = collider;
                    return true;
                }
                else return false;
            }

            if (collider.InCollider(Position))
            {
                var closestPoint = collider.ClosestPoint(Position, out var straight, Radius);
                collision.Normal = straight.Normal;
                collision.CorrectPointAfterCollision = closestPoint;
                collision.Reflection = closestPoint;
                collision.ContactPoints = new List<Vector2>();
                collision.ContactPoints.Add(closestPoint);
                collision.OtherCollider = collider;
                return true;
            }


            var ray = new Ray(Position, moveDirection);
            if (collider.RayCast(ray, out var nearestPoint, out var farthestPoint, Radius))
            {
                var oldPositionNearestDiff = Position - nearestPoint;
                if (oldPositionNearestDiff.sqrMagnitude <= moveDirection.sqrMagnitude)
                {
                    var closestPoint = collider.ClosestPoint(Position, out var straight, Radius);
                    collision.Normal = straight.Normal;
                    collision.CorrectPointAfterCollision = nearestPoint; // closestPoint;
                    collision.Reflection = nearestPoint +
                                           Vector2.Reflect(ray.Direction + (Position - nearestPoint), straight.Normal);
                    collision.ContactPoints = new List<Vector2>();
                    collision.ContactPoints.Add(nearestPoint);
                    collision.OtherCollider = collider;
                    return true;
                }

                return false;
            }

            return false;
        }


        public bool InCollider(Vector2 point, float radius = 0)
        {
            var diff = (point - Position).sqrMagnitude;
            if (diff <= (radius + Radius) * (radius + Radius))
                return true;
            return false;
        }

        public Vector2 ClosestPoint(Vector2 point, out Straight straight, float radius = 0)
        {
            var direction = (point - Position);
            if (direction.sqrMagnitude == 0)
            {
                direction = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized;
            }

            var point1 = Position + direction.normalized * (radius + Radius);
            straight = new Straight(point1, point1 - Vector2.Perpendicular(direction.normalized));
            return direction.normalized * (Radius + radius);
        }

        public bool RayCast(Ray ray, out Vector2 nearestPoint, out Vector2 farthestPoint, float radius = 0)
        {
            var bigRadius = Radius + radius;
            var direction = ray.Direction.normalized;
            var a = direction.x * direction.x + direction.y * direction.y;
            var b = -2 * Position.x * direction.x + 2 * ray.Position.x * direction.x -
                2 * Position.y * direction.y + 2 * ray.Position.y * direction.y;
            var c = Position.x * Position.x - 2 * Position.x * ray.Position.x + ray.Position.x * ray.Position.x +
                    Position.y * Position.y - 2 * Position.y * ray.Position.y + ray.Position.y * ray.Position.y -
                    bigRadius * bigRadius; //Radius*Radius+radius*radius;//
            var desc = b * b - 4 * a * c;
            if (desc >= 0)
            {
                var t1 = (-b + Mathf.Sqrt(desc)) / 2 / a;
                var t2 = (-b - Mathf.Sqrt(desc)) / 2 / a;
                if (t1 < 0 && t2 < 0)
                {
                    nearestPoint = farthestPoint = Vector2.zero;
                    return false;
                }

                if (Mathf.Abs(t1) >= Mathf.Abs(t2))
                {
                    nearestPoint = ray.Position + direction * t2;
                    farthestPoint = ray.Position + direction * t1;
                }
                else
                {
                    nearestPoint = ray.Position + direction * t1;
                    farthestPoint = ray.Position + direction * t2;
                }

                return true;
            }

            nearestPoint = farthestPoint = Vector2.zero;
            return false;
        }
    }

    public interface ICollider
    {
        List<Vector2> Points { get; }
        List<Straight> Sides { get; }
        float Radius { get; }
        Vector2 Position { get; set; }

        bool PointCollision(Vector2 newPos, ICollider collider, out Collision collision);
        bool InCollider(Vector2 point, float radius = 0);
        Vector2 ClosestPoint(Vector2 point, out Straight straight, float radius = 0);
        bool RayCast(Ray ray, out Vector2 nearestPoint, out Vector2 farthestPoint, float radius = 0);
    }


    public static class Geometry
    {
        /*public static bool CalculateCircleIntersection(Straight intersectStraight, Ray ray, float Radius,
            out Vector2 closestPosition, out Vector2 restDistance, out Vector2 contactPoints)
        {
            if (intersectStraight.RayIntersect(ray, out float d))
            {
                var previousDistance = intersectStraight.DistanceToPoint(ray.Position);
                var nextDistance = intersectStraight.DistanceToPoint(ray.Position + ray.Direction);
                if (nextDistance < Radius && previousDistance > nextDistance)
                {
                    var verticalComponent =
                        -previousDistance * intersectStraight.Normal + Radius * intersectStraight.Normal; //
                    var cos = Vector2.Dot(verticalComponent.normalized, ray.Direction.normalized);
                    if (cos == 0 || ray.Direction.sqrMagnitude <= 0.001f)
                    {
                        contactPoints = closestPosition = restDistance = Vector2.zero;
                        if (cos == 0)
                            return false;
                        else return true;
                    }

                    closestPosition = ray.Direction.normalized * verticalComponent.magnitude / cos;
                    restDistance = Vector2.Reflect(ray.Direction - closestPosition, intersectStraight.Normal);
                    contactPoints = ray.Position + closestPosition - intersectStraight.Normal * Radius;
                    return true;
                }
                else
                {
                    contactPoints = closestPosition = restDistance =
                        -intersectStraight.DistanceToPoint(ray.Position) * intersectStraight.Normal;
                    return false;
                }
            }
            else
            {
                contactPoints = closestPosition =
                    restDistance = -intersectStraight.DistanceToPoint(ray.Position) * intersectStraight.Normal;
                return false;
            }
        }*/
    }
}
