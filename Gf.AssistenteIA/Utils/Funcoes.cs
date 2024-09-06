namespace Gf.AssistenteIA.Utils
{
      public static class Funcoes
      {
            public static bool CBool(object? valor)
            {
                  bool v = false;
                  if (!Equals(valor, DBNull.Value) && valor != null)
                  {
                        try
                        {
                              if (valor.GetType() == typeof(string))
                              {
                                    if ((valor as string).ToLower().Equals("false")
                                        || (valor as string).ToLower().Equals("no")
                                        || (valor as string).ToLower().Equals("nao")
                                        || (valor as string).ToLower().Equals("não"))
                                    {
                                          valor = false;
                                    }
                                    else if ((valor as string).ToLower().Equals("true")
                                       || (valor as string).ToLower().Equals("yes")
                                       || (valor as string).ToLower().Equals("sim"))
                                    {
                                          valor = true;
                                    }
                                    else
                                    {
                                          valor = CInt(valor);
                                    }
                              }
                              else if (valor.GetType() == typeof(float))
                              {
                                    valor = CInt(valor);
                              }

                              v = Convert.ToBoolean(valor);
                        }
                        catch
                        {
                              v = false;
                        }
                  }

                  return v;
            }
            /// <summary>
            /// Retorna 0 por padrão
            /// </summary>
            /// <param name="valor"></param>
            /// <returns></returns>
            public static int CInt(object valor)
            {
                  return (int)CInt(valor, 0)!;
            }
            public static int? CInt(object valor, int? padrao = 0)
            {
                  int? v = padrao;
                  if (!Equals(valor, DBNull.Value) && valor != null)
                  {
                        string s = CStr(valor, "")!;
                        if (s.Contains(".") && s.Contains(","))
                        {
                              if (s.IndexOf(".") > s.IndexOf(","))
                              {
                                    s = s.Replace(",", "").Replace(".", ",");
                              }
                              else
                              {
                                    s = s.Replace(".", "");
                              }

                        }
                        else if (s.Contains("."))
                        {
                              s = s.Replace(".", ",");
                        }
                        if (s.Contains(".") || s.Contains(","))
                        {
                              valor = CDec(s);
                        }
                        else
                        {
                              valor = s;
                        }
                        try
                        {
                              v = Convert.ToInt32(valor);
                        }
                        catch { }
                  }

                  return v;
            }
            public delegate object FuncReturn<in T>(T obj);
            [System.Diagnostics.DebuggerHidden]
            public static object WithF<T>(this T item, FuncReturn<T> work)
            {
                  return (T)work(item);
            }
            [System.Diagnostics.DebuggerHidden]
            public static void With<T>(this T item, Action<T> work)
            {
                  work(item);
            }
            public static decimal CDec(object? valor, decimal padrao = (decimal)0.0)
            {
                  decimal v = padrao;
                  if (!Equals(valor, DBNull.Value) && valor != null)
                  {
                        if (valor.GetType() == typeof(decimal))
                        {
                              return (decimal)valor;
                        }

                        string s = valor.ToString()!;
                        if (s.Contains(".") && s.Contains(","))
                        {
                              if (s.IndexOf(".") > s.IndexOf(","))
                              {
                                    s = s.Replace(",", "").Replace(".", ",");
                              }
                              else
                              {
                                    s = s.Replace(".", "");
                              }

                        }
                        else if (s.Contains("."))
                        {
                              s = s.Replace(".", ",");
                        }
                        try
                        {
                              decimal.TryParse(s, out v);
                        }
                        catch { }
                  }

                  return v;
            }
            public static string? CStr(object obj, string? padrao = null, int MaxLength = 0)
            {
                  string? v = padrao;
                  if (obj != null && !Equals(obj, DBNull.Value))
                  {
                        v = Convert.ToString(obj);
                        if (IsNullOrEmpty(v))
                        {
                              v = padrao;
                        }
                  }
                  if (MaxLength > 0 && v != null && v?.Length > MaxLength)
                  {
                        v = v.Substring(0, MaxLength);
                  }

                  return v;
            }
            public static bool IsNullOrEmpty(object? valor)
            {
                  bool v = false;
                  if (valor == null)
                  {
                        v = true;
                  }
                  else if (Equals(valor, DBNull.Value))
                  {
                        v = true;
                  }
                  else if (valor.GetType() == typeof(string))
                  {
                        v = (valor as string == "");
                  }

                  return v;
            }

      }
}
