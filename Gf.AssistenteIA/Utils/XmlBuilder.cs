using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gf.AssistenteIA.Utils
{
      public class XmlBuilder
      {
            public XmlBuilder(string nomeTagRaiz)
            {
                  raiz = new XmlElemento(nomeTagRaiz);
            }
            public XmlBuilder(XmlDocument doc)
            {
                  string nomeTagRaiz = doc.DocumentElement?.Name;

                  raiz = new XmlElemento(nomeTagRaiz);

                  AddElementNodes(doc.DocumentElement, raiz);

            }
            public static XmlBuilder FromFile(string file)
            {
                  if (!File.Exists(file)) return null;

                  string xml = File.ReadAllText(file, Encoding.UTF8);

                  return FromXml(xml);
            }
            public static XmlBuilder FromXml(string xml)
            {
                  if (xml == null || xml.Length < 1) return null;
                  var doc = new XmlDocument();
                  doc.LoadXml(xml);

                  var x = new XmlBuilder(doc);

                  return x;
            }
            public XmlElemento raiz { get; private set; }

            internal void AddElementNodes(System.Xml.XmlNode node, XmlElemento elemento)
            {
                  if (node.Attributes.Count > 0)
                  {
                        foreach (System.Xml.XmlAttribute at in node.Attributes)
                              elemento.AddAtributo(at.Name, at.Value);
                  }
                  if (node.ChildNodes.Count > 0)
                  {
                        foreach (System.Xml.XmlNode n in node.ChildNodes)
                        {
                              if (n.GetType() == typeof(System.Xml.XmlElement))
                              {
                                    var e = elemento.AddElemento(n.Name);
                                    AddElementNodes(n, e);
                              }
                              else if (n.GetType() == typeof(System.Xml.XmlText))
                              {
                                    System.Xml.XmlText n1 = (System.Xml.XmlText)n;
                                    elemento.valor = n1.Value;
                              }
                        }
                  }
                  else if (node.GetType() == typeof(System.Xml.XmlElement))
                  {
                        System.Xml.XmlElement n1 = (System.Xml.XmlElement)node;
                        elemento.addTagSeValorVazio = !n1.IsEmpty;
                        elemento.valor = (n1.IsEmpty) ? null : "";
                  }
                  else
                  {
                        elemento.addTagSeValorVazio = true;
                        elemento.valor = node.Value;
                  }
            }

            public bool Save(string file, bool xmlDeclaracao = false)
            {
                  try
                  {
                        string s = this.ToString(xmlDeclaracao);

                        File.WriteAllText(file, s, Encoding.UTF8);
                        return true;
                  }
                  catch
                  {
                        return false;
                  }

            }

            public string ToString(bool xmlDeclaracao = false)
            {
                  StringBuilder sb = new StringBuilder();
                  if (xmlDeclaracao)
                  {
                        sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                  }
                  sb.Append(raiz.ToXmlString()?.Replace("&", "&amp;"));
                  return sb.ToString();
            }
            public class eRef
            {
                  public eRef(XmlElemento parent) : base()
                  {
                        this.parent = parent;
                  }
                  XmlElemento parent;
                  public XmlElemento this[string nomeTag, int index = 0]
                  {
                        get
                        {
                              var l = parent.itens.Where(r => r.nomeTag == nomeTag).ToArray();
                              if (index > l.Length || l.Length == 0)
                              {
                                    return new XmlElemento(nomeTag);
                              }
                              else
                              {
                                    return l[index];
                              }
                        }
                  }
            }
            public class aRef
            {
                  public aRef(XmlElemento parent) : base()
                  {
                        this.parent = parent;
                  }
                  XmlElemento parent;
                  public XmlAtributo this[string nomeAtributo, int index = 0]
                  {
                        get
                        {
                              var l = parent.atributos.Where(r => r.nomeAtributo == nomeAtributo).ToArray();
                              if (index > l.Length)
                              {
                                    return null;
                              }
                              else
                              {
                                    return l[index];
                              }
                        }
                  }
            }
            public class XmlElemento
            {
                  public XmlElemento()
                  {
                        atributos = new List<XmlAtributo>();
                        itens = new List<XmlElemento>();
                        e = new eRef(this);
                        a = new aRef(this);
                  }
                  public eRef e { get; }
                  public aRef a { get; }
                  public XmlElemento(string nomeTag)
                  {
                        atributos = new List<XmlAtributo>();
                        itens = new List<XmlElemento>();
                        e = new eRef(this);
                        a = new aRef(this);

                        this.nomeTag = nomeTag;
                        this.addTagSeValorNull = true;
                  }
                  public XmlElemento(string nomeTag, string valor)
                  {
                        atributos = new List<XmlAtributo>();
                        itens = new List<XmlElemento>();
                        e = new eRef(this);


                        this.nomeTag = nomeTag;
                        this.valor = valor;
                  }
                  public XmlElemento(string nomeTag, string valor, bool addTagSeValorNull)
                  {
                        atributos = new List<XmlAtributo>();
                        itens = new List<XmlElemento>();
                        e = new eRef(this);


                        this.nomeTag = nomeTag;
                        this.valor = valor;
                        this.addTagSeValorNull = addTagSeValorNull;
                  }
                  public XmlElemento(string nomeTag, XmlElemento elemento)
                  {
                        atributos = new List<XmlAtributo>();
                        itens = new List<XmlElemento>();
                        e = new eRef(this);

                        this.nomeTag = nomeTag;
                        elemento.parent_ = this;
                        this.itens.Add(elemento);
                  }
                  public XmlAtributo AddAtributo(string nomeAtributo, string valor)
                  {
                        if (valor == null)
                        {
                              valor = null;
                        }
                        XmlAtributo x = new XmlAtributo(this, nomeAtributo, valor);

                        this.atributos.Add(x);

                        return x;
                  }
                  public XmlElemento AddFromXml(string xml)
                  {
                        this.valor = null;

                        var doc = XmlBuilder.FromXml(xml);
                        doc.raiz.parent_ = this;

                        this.itens.Add(doc.raiz);

                        return doc.raiz;
                  }
                  public XmlElemento AddElemento(string nomeTag)
                  {
                        XmlElemento x = new XmlElemento(nomeTag);
                        x.parent_ = this;

                        this.itens.Add(x);

                        return x;
                  }
                  public XmlElemento AddElemento(string nomeTag, object? valor)
                  {
                        if (valor == null || valor == DBNull.Value)
                        {
                              valor = null;
                        }
                        XmlElemento x = new XmlElemento(nomeTag, valor?.ToString());
                        x.parent_ = this;

                        this.itens.Add(x);

                        return x;
                  }
                  public XmlElemento AddElemento(string nomeTag, object valor, bool addTagSeValorNull)
                  {
                        if (!addTagSeValorNull && (valor == null || valor == DBNull.Value))
                        {
                              return null;
                        }
                        XmlElemento x = new XmlElemento(nomeTag, valor?.ToString(), addTagSeValorNull);
                        x.parent_ = this;

                        this.itens.Add(x);

                        return x;
                  }
                  public XmlElemento AddElemento(string nomeTag, object valor, bool addTagSeValorNull, bool addTagSeValorVazio)
                  {
                        if (!addTagSeValorNull && (valor == null || valor == DBNull.Value))
                        {
                              return null;
                        }
                        if (!addTagSeValorVazio && (valor == null || valor == DBNull.Value || "".Equals(valor)))
                        {
                              return null;
                        }
                        XmlElemento x = new XmlElemento(nomeTag, valor?.ToString(), addTagSeValorNull);
                        x.parent_ = this;

                        this.itens.Add(x);

                        return x;
                  }
                  public XmlElemento AddXmlElementoInto(string intoTag, XmlElemento elemento)
                  {
                        XmlElemento x = GetElemento(intoTag);
                        if (x == null)
                        {
                              x = new XmlElemento(intoTag);
                              x.parent_ = this;
                              this.itens.Add(x);
                        }

                        elemento.parent_ = x;
                        x.itens.Remove(elemento);
                        x.itens.Add(elemento);

                        return elemento;
                  }
                  public XmlElemento AddXmlElemento(string nomeTag, XmlElemento elemento)
                  {
                        XmlElemento x = new XmlElemento(nomeTag, elemento);
                        x.parent_ = this;

                        this.itens.Add(x);

                        return x;
                  }
                  public XmlElemento AddXmlString(string nomeTag, string xml)
                  {
                        XmlElemento x = new XmlElemento(nomeTag);
                        x.addTag = false;
                        x.parent_ = this;
                        x.valor = xml;

                        this.itens.Add(x);

                        return x;
                  }

                  public List<string> tagsOrdem { get; } = new List<string>();

                  /// <summary>
                  /// get retorna null caso não encontrar o elemento
                  /// set cria um novo elemento se não existir
                  /// </summary>
                  /// <param name="nomeTag"></param>
                  /// <returns></returns>
                  public string this[string nomeTag]
                  {
                        get
                        {
                              return GetElemento(nomeTag)?.valor;
                        }
                        set
                        {
                              var x = GetElemento(nomeTag);
                              if (x is null)
                              {
                                    x = new XmlElemento(nomeTag, value?.ToString());
                                    x.parent_ = this;

                                    this.itens.Add(x);
                              }
                              else
                              {
                                    x.valor = value;
                              }
                        }
                  }

                  public XmlElemento GetElemento(string nomeTag)
                  {
                        XmlElemento x = null;

                        foreach (XmlElemento item in this.itens)
                        {
                              if (item.nomeTag.Equals(nomeTag))
                              {
                                    x = item;
                                    break;
                              }
                        }

                        return x;
                  }

                  public XmlElemento Update(string nomeTag, object valor)
                  {
                        if (valor == DBNull.Value)
                        {
                              valor = null;
                        }
                        XmlElemento x = null;

                        foreach (XmlElemento item in this.itens)
                        {
                              if (item.nomeTag.Equals(nomeTag))
                              {
                                    x = item;
                                    x.valor = valor?.ToString();
                                    break;
                              }
                        }

                        return x;
                  }

                  private XmlElemento parent_;
                  public XmlElemento parent
                  {
                        get
                        {
                              return parent_;
                        }
                  }

                  private bool _addTag = true;
                  public bool addTag
                  {
                        get
                        {
                              return _addTag;
                        }
                        set
                        {
                              _addTag = value;
                        }
                  }
                  private bool _addTagSeValorNull = false;
                  public bool addTagSeValorNull
                  {
                        get
                        {
                              return _addTagSeValorNull;
                        }
                        set
                        {
                              _addTagSeValorNull = value;
                        }
                  }
                  private bool _addTagSeValorVazio = false;
                  public bool addTagSeValorVazio
                  {
                        get
                        {
                              return _addTagSeValorVazio;
                        }
                        set
                        {
                              _addTagSeValorVazio = value;
                        }
                  }
                  public string nomeTag { get; set; }
                  private string valor_;
                  public string valor // deverá ser valor OU itens
                  {
                        get
                        {
                              return valor_;
                        }
                        set
                        {
                              if (itens.Count > 0)
                              {
                                    throw (new InvalidOperationException("Não pode aplicar valor quando há ítens na coleção \'itens As List(Of XmlElement)\'"));
                              }
                              valor_ = value;
                        }
                  }
                  public List<XmlAtributo> atributos { get; set; }
                  public List<XmlElemento> itens { get; set; }
                  public List<XmlElemento> all_itens
                  {
                        get
                        {
                              List<XmlElemento> l = new List<XmlElemento>();
                              l.AddRange(itens);
                              foreach (var i in itens)
                                    l.AddRange(i.all_itens);

                              return l;
                        }
                  }

                  public void OrdenarTags()
                  {
                        if (tagsOrdem.Count > 0)
                        {
                              var l = new List<XmlElemento>();
                              foreach (var t in tagsOrdem)
                              {
                                    var x = itens.FirstOrDefault(i => i.nomeTag == t);
                                    if (x != null)
                                    {
                                          l.Add(x);
                                          itens.Remove(x);
                                    }
                                    if (itens.Count == 0) break;
                              }
                              l.AddRange(itens);
                              itens = l;
                        }
                  }

                  public string AtributosToString()
                  {
                        StringBuilder sb = new StringBuilder();
                        foreach (XmlAtributo item in this.atributos)
                        {
                              sb.Append($" {item.ToString()}");
                        }

                        return sb.ToString();
                  }
                  public string ToXmlString()
                  {
                        StringBuilder sb = new StringBuilder();
                        if (itens.Count > 0)
                        {
                              this.OrdenarTags();

                              sb.Append($"<{nomeTag}{this.AtributosToString()}>");
                              foreach (XmlElemento item in this.itens)
                              {
                                    sb.Append(item.ToXmlString());
                              }
                              sb.Append($"</{nomeTag}>");
                        }
                        else
                        {
                              if (!addTagSeValorNull && this.valor == null)
                              {
                                    return null;
                              }
                              if (!addTagSeValorVazio && "".Equals(this.valor))
                              {
                                    return null;
                              }
                              if (addTag)
                              {
                                    sb.Append($"<{nomeTag}{this.AtributosToString()}>{this.valor}</{nomeTag}>");
                              }
                              else
                              {
                                    sb.Append($"{this.valor}");
                              }
                        }

                        return sb.ToString();
                  }
                  public new string ToString()
                  {
                        return this.ToXmlString();
                  }
            }
            public class XmlAtributo
            {
                  public XmlAtributo(XmlElemento parent, string nomeAtributo)
                  {

                        this.parent_ = parent;
                        this.nomeAtributo = nomeAtributo;
                  }
                  public XmlAtributo(XmlElemento parent, string nomeAtributo, string valor)
                  {

                        this.parent_ = parent;
                        this.nomeAtributo = nomeAtributo;
                        this.valor = valor;
                  }
                  private XmlElemento parent_;
                  public XmlElemento parent
                  {
                        get
                        {
                              return parent_;
                        }
                  }
                  public string nomeAtributo { get; set; }
                  public string valor { get; set; }

                  public new string ToString()
                  {
                        return $"{nomeAtributo}=\"{valor}\"";
                  }
            }
      }
}
