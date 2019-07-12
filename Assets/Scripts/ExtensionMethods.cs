using System;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods {
    public class Pair<T, U> {
        public Pair() {
        }

        public Pair(T first, U second) {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };

    /*public class Pair2 {
   

        public Pair2(bool first, Action second) {
            this.First = first;
            this.Second = second;
        }

        public bool First { get; set; }
        public Action Second { get; set; }
    };*/

    public static class MyExtensions {

        public static String toString(this Vector3[] arrayVector) {
            String ret = "";
            foreach(Vector3 v in arrayVector) {
                ret += v + ", ";
            }

            return ret;
        }

        public static bool Contains(this Vector3[] arrayVector, Vector3 containedVector) {
            bool ret = false;

            foreach(Vector3 v in arrayVector) {
                if (v.isAproximated(containedVector, 0.1f)) {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        public static void Times(this int num, Action action) {
            for(int i = 0; i <= num; i++) {
                action();
            }
        }

        public static bool isEqual(this Vector3 v1, Vector3 v2, int range) {
            int x1 = Mathf.RoundToInt(v1.x * range);
            int y1 = Mathf.RoundToInt(v1.y * range);
            int z1 = Mathf.RoundToInt(v1.z * range);
            int x2 = Mathf.RoundToInt(v2.x * range);
            int y2 = Mathf.RoundToInt(v2.y * range);
            int z2 = Mathf.RoundToInt(v2.z * range);
            return x1 == x2 && y1 == y2 && z1 == z2;
        }

        public static bool isAproximated(this Vector3 v1, Vector3 v2, float aprox) {
            return (v1.x >= v2.x - aprox && v1.x <= v2.x + aprox) && (v1.y >= v2.y - aprox && v1.y <= v2.y + aprox) && (v1.z >= v2.z - aprox && v1.z <= v2.z + aprox);
        }

        public delegate void ParameterFunction();
        public static void When(Vector3 v, Pair<bool, Action>[] operations) {
            foreach (Pair<bool, Action> operation in operations) {
                if (operation.First == true) {
                    operation.Second.Invoke();
                    break;
                    /*var operation = operations.Second;
                    var param = operation.Parameters[0];

                    var body = operation.Body as MethodCallExpression;

                    if (body == null || body.Object != param || body.Method.Name != "UserAuthentication") {
                        throw new NotSupportedException();
                    }

                    object requestValue;

                    var constantExpression = body.Arguments[0] as ConstantExpression;

                    if (constantExpression == null)
                        requestValue = Expression.Lambda<Func<object>>(body.Arguments[0]).Compile()();
                    else
                        requestValue = constantExpression.Value;

                    requestValue;*/
                }
            }
        }
        /*public static void When<T>(Vector3 v, Expression<Func<bool, T>> operation) {
            //foreach(Expression<Func<T, bool>> operation in funcs) {
                var param = operation.Parameters[0];

                var body = operation.Body as MethodCallExpression;

                if (body == null || body.Object != param || body.Method.Name != "UserAuthentication") {
                    throw new NotSupportedException();
                }

                object requestValue;

                var constantExpression = body.Arguments[0] as ConstantExpression;

                if (constantExpression == null)
                    requestValue = Expression.Lambda<Func<object>>(body.Arguments[0]).Compile()();
                else 
                    requestValue = constantExpression.Value;

                bool? b = requestValue as bool?;
                if (b != null && b == true) {
                    operation.Compile();
                  // break;
                }
           // }
        }*/
    }
}
/*
namespace CommonCode {
    public class CommonCode {
        public delegate void ParameterFunction();
        public static void When(Vector3 v, HashSet<KeyValuePair<bool, ParameterFunction>> h) {
            foreach (KeyValuePair<bool, ParameterFunction> p in h) {
                if (p.Key) {
                    p.Value();
                    break;
                }
            }
        }
    }
}*/