using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Native.Tool.IniConfig.Linq
{
	/// <summary>
	/// 描述配置项 (Ini) 文件的类
	/// </summary>
	public sealed class IObject : ICollection<ISection>, IEquatable<IObject>
	{
		#region --字段--
		private readonly SortedList<string, ISection> _sortedList;
		private readonly string _fileName;
		private bool _readOnly = false;
		private Encoding _encoding = Encoding.Default;
		private static readonly Lazy<Regex[]> regices = new Lazy<Regex[]>(() => new Regex[]
	   {
			new Regex(@"^\[(.+)\]", RegexOptions.Compiled),						//匹配 节
			new Regex(@"^([^\r\n=]+)=((?:[^\r\n]+)?)",RegexOptions.Compiled),    //匹配 键值对
 			new Regex(@"^;(?:[\s\S]*)", RegexOptions.Compiled)              //匹配 注释
	   });
		/// <summary>
		/// 获取用于解析 Ini 配置项的 Regex 对象数组
		/// </summary>
		private static Regex[] Regices { get { return regices.Value; } }
		#endregion

		#region --属性--
		/// <summary>
		/// 获取或设置与指定的键关联的值
		/// </summary>
		/// <param name="key">要获取或设置其值的键</param>
		/// <exception cref="ArgumentException">key 与传入的 <see cref="ISection.SectionName"/> 不同</exception>
		/// <exception cref="ArgumentNullException">key 为 null</exception>
		/// <exception cref="KeyNotFoundException">已检索该属性且集合中不存在 key</exception>
		/// <returns>与指定的键相关联的值。 如果找不到指定的键，则 get 操作会引发一个 <see cref="KeyNotFoundException"/>，而 set 操作会创建一个使用指定键的新元素</returns>
		public ISection this[string key]
		{
			get
			{
				if (!this._sortedList.ContainsKey (key))
				{
					this._sortedList.Add (key, new ISection (key));
				}
				return this._sortedList[key];
			}
			set
			{
				if (!key.Equals(value.SectionName))
				{
					throw new ArgumentException (string.Format("传入的节点对象 SectionName: {0} 与当前索引器的 Key: {1} 不符", value.SectionName, key));
				}
				this._sortedList[key] = value;
			}
		}
		/// <summary>
		/// 获取当前实例的文件名
		/// </summary>
		public string FileName
		{
			get { return this._fileName; }
		}
		/// <summary>
		/// 获取包含在 <see cref="IObject"/> 中的 <see cref="ISection"/> 的数目
		/// </summary>
		public int Count
		{
			get { return this._sortedList.Count; }
		}
		/// <summary>
		/// 获取一个值，该值指示 <see cref="IObject"/> 是否为只读
		/// </summary>
		public bool IsReadOnly
		{
			get { return this._readOnly; }
		}
		/// <summary>
		/// 获取或设置用于读取或保存 Ini 配置项的 <see cref="System.Text.Encoding"/> 实例, 默认: ANSI
		/// </summary>
		public Encoding Encoding { get { return this._encoding; } set { this._encoding = value; } }

		/// <summary>
		/// 获取或设置用于保存 Ini 配置项的 <see cref="Uri"/> 实例
		/// </summary>
		public Uri Path { get; set; }
		#endregion

		#region --构造函数--


		/// <summary>
		/// 初始化 <see cref="IObject"/> 类的新实例，该实例为空并且具有默认初始容量
		/// </summary>
		/// <param name="fileName"><see cref="IObject"/> 使用的文件名</param>
		public IObject (string fileName)
		{
			this._fileName = fileName;
			this._sortedList = new SortedList<string, ISection> ();
		}
		/// <summary>
		/// 初始化 <see cref="IObject"/> 类的新实例，该实例为空并且具有指定的初始容量
		/// </summary>
		/// <param name="fileName"><see cref="IObject"/> 使用的文件名</param>
		/// <param name="capacity">新列表最初可以存储的元素数</param>
		public IObject (string fileName, int capacity)
		{
			this._fileName = fileName;
			this._sortedList = new SortedList<string, ISection> (capacity);
		}
		/// <summary>
		/// 初始化 <see cref="IObject"/> 类的新实例，该实例包含从指定集合复制的元素并且具有足够的容量来容纳所复制的元素
		/// </summary>
		/// <param name="fileName"><see cref="IObject"/> 使用的文件名</param>
		/// <param name="collection">一个集合，其元素被复制到新列表中</param>
		public IObject (string fileName, IEnumerable<ISection> collection)
		{
			this._fileName = fileName;
			this._sortedList = new SortedList<string, ISection> (collection.ToDictionary (keySelector => keySelector.SectionName));
		}
		#endregion

		#region --公开方法--
		/// <summary>
		/// 从文件以 ANSI 编码创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>转换成功返回 IniObject 实例对象</returns>
		public static IObject Load(string filePath)
		{
			return Load(new Uri(filePath));
		}
		/// <summary>
		/// 从文件以 ANSI 编码创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="fileUri">文件路径的 Uri 对象</param>
		/// <returns>转换成功返回 IniObject 实例对象</returns>
		public static IObject Load(Uri fileUri)
		{
			return Load(fileUri, Encoding.Default);
		}
		/// <summary>
		/// 从文件以指定编码创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="filePath">文件路径字符串</param>
		/// <param name="encoding">文件编码</param>
		/// <returns></returns>
		public static IObject Load(string filePath, Encoding encoding)
		{
			return Load(new Uri(filePath), encoding);
		}
		/// <summary>
		/// 从文件以指定编码创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="fileUri">文件路径的 Uri 对象</param>
		/// <param name="encoding">文件编码</param>
		/// <returns>转换成功返回 IniObject 实例对象</returns>
		public static IObject Load(Uri fileUri, Encoding encoding)
		{
			fileUri = ConvertAbsoluteUri(fileUri);

			if (!fileUri.IsFile)
			{
				throw new ArgumentException("所指定的必须是文件 URI", "fileUri");
			}

			//解释 Ini 文件
			using (TextReader textReader = new StreamReader(fileUri.GetComponents(UriComponents.Path, UriFormat.Unescaped), encoding))
			{
				IObject iObj = ParseIni(textReader, fileUri.GetComponents(UriComponents.Path, UriFormat.Unescaped));
				iObj.Path = fileUri;
				return iObj;
			}
		}
		/// <summary>
		/// 将 Ini 配置项保存到指定的文件。如果存在指定文件，则此方法会覆盖它<para>默认: ANSI 编码保存</para>
		/// </summary>
		public void Save()
		{
			if (string.IsNullOrEmpty(this._fileName))
			{
				throw new UriFormatException(string.Format("Uri: {0}, 是无效的 Uri 对象", "IniObject.Path"));
			}
			this.Save(new Uri(this._fileName));
		}
		/// <summary>
		/// 将 Ini 配置项保存到指定的文件。 如果存在指定文件，则此方法会覆盖它。
		/// </summary>
		/// <param name="fileUri">要将文档保存到其中的文件的位置。</param>
		private void Save(Uri fileUri)
		{
			fileUri = ConvertAbsoluteUri(fileUri);

			if (!fileUri.IsFile)
			{
				throw new ArgumentException("所指定的必须是文件 URI", "fileUri");
			}
			using (TextWriter textWriter = new StreamWriter(fileUri.GetComponents(UriComponents.Path, UriFormat.Unescaped), false, this.Encoding))
			{
				foreach (ISection section in this)
				{
					textWriter.WriteLine("[{0}]", section.SectionName);
					foreach (KeyValuePair<string, IValue> pair in section)
					{
						textWriter.WriteLine("{0}={1}", pair.Key, pair.Value);
					}
					textWriter.WriteLine();
				}
			}
		}
		//=========================

		/// <summary>
		/// 确定两个指定的 <see cref="IObject"/> 对象是否具有相同的值
		/// </summary>
		/// <param name="objA">要比较的第一个 <see cref="IContainer{TValue}"/>，或 <see langword="null"/></param>
		/// <param name="objB">要比较的第二个 <see cref="IContainer{TValue}"/>，或 <see langword="null"/></param>
		/// <returns>如果 <see langword="true"/> 的值与 objA" 的值相同，则为 objB；否则为 <see langword="false"/>。 如果 objA 和 objB 均为 <see langword="null"/>，此方法将返回 <see langword="true"/></returns>
		public static bool Equals (IObject objA, IObject objB)
		{
			if (objA == objB)
			{
				return true;
			}

			if (!(objA.Count == objB.Count) || objA == null || objB == null)
			{
				return false;
			}

			for (int i = 0; i < objA.Count; i++)
			{
				ISection valueA = objA._sortedList.Values.ElementAt (i);
				ISection valueB = objB._sortedList.Values.ElementAt (i);

				if (!object.Equals (valueA, valueB))
				{
					return false;
				}
			}

			return true;
		}
		/// <summary>
		/// 将某项添加到 <see cref="IObject"/> 中
		/// </summary>
		/// <param name="item">要添加到 <see cref="IObject"/> 的对象</param>
		/// <exception cref="NotSupportedException"><see cref="IObject"/> 为只读</exception>
		public void Add (ISection item)
		{
			this._sortedList.Add (item.SectionName, item);
		}
		/// <summary>
		/// 从 <see cref="IObject"/> 中移除所有项
		/// </summary>
		/// <exception cref="NotSupportedException"><see cref="IObject"/> 为只读</exception>
		public void Clear ()
		{
			this._sortedList.Clear ();
		}
		/// <summary>
		/// 确定 <see cref="IObject"/> 是否包含特定值
		/// </summary>
		/// <param name="item">要在 <see cref="IObject"/> 中定位的对象</param>
		/// <returns>如果在 <see langword="true"/> 中找到 item，则为 <see cref="IObject"/>；否则为 <see langword="false"/></returns>
		public bool Contains (ISection item)
		{
			return this._sortedList.ContainsValue (item);
		}
		/// <summary>
		/// 确定 <see cref="IObject"/> 是否包含特定名称的 <see cref="ISection"/>
		/// </summary>
		/// <param name="key">要在 <see cref="IObject"/> 中定位的键</param>
		/// <exception cref="ArgumentNullException">key 为 null</exception>
		/// <returns>如果 <see langword="true"/> 包含具有指定键的元素，则为 <see cref="IObject"/>；否则为 <see langword="false"/></returns>
		public bool ContainsKey (string key)
		{
			return this._sortedList.ContainsKey (key);
		}
		/// <summary>
		/// 从特定的 <see cref="IObject"/> 索引处开始，将 System.Array 的元素复制到一个 <see cref="Array"/> 中
		/// </summary>
		/// <param name="array">一维 <see cref="Array"/>，它是从 <see cref="IObject"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引</param>
		/// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制</param>
		/// <exception cref="ArgumentNullException">array 为 null</exception>
		/// <exception cref="ArgumentOutOfRangeException">arrayIndex 小于 0</exception>
		/// <exception cref="ArgumentException">源中的元素数目 <see cref="IObject"/> 大于从的可用空间 arrayIndex 目标从头到尾 array</exception>
		public void CopyTo (ISection[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException ("array");
			}

			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException ("arrayIndex", "arrayIndex 不能小于 0");
			}

			if (arrayIndex >= array.Length && arrayIndex != 0)
			{
				throw new ArgumentException ("arrayIndex 等于或大于数组的长度。");
			}

			if (this._sortedList.Count > array.Length - arrayIndex)
			{
				throw new ArgumentException ("源 IObject 中的元素数量大于从 arrayIndex 到目标数组末尾的可用空间");
			}

			int num = 0;
			foreach (KeyValuePair<string, ISection> item in this._sortedList)
			{
				array[arrayIndex + num] = item.Value;
				num++;
			}
		}
		/// <summary>
		/// 从 <see cref="IObject"/> 中移除指定名称的 <see cref="ISection"/>
		/// </summary>
		/// <param name="key">要移除的元素的键</param>
		/// <exception cref="ArgumentNullException">item 为 null</exception>
		/// <returns>如果该元素已成功移除，则为 <see langword="true"/>；否则为 <see langword="false"/>。 如果在原始 <see langword="true"/> 中没有找到 key，则此方法也会返回 <see cref="IObject"/>。</returns>
		public bool Remove (string key)
		{
			return _sortedList.Remove (key);
		}
		/// <summary>
		/// 从 <see cref="IObject"/> 中移除指定的元素
		/// </summary>
		/// <param name="item">要移除的元素</param>
		/// <exception cref="ArgumentNullException">item 为 null</exception>
		/// <returns>如果该元素已成功移除，则为 <see langword="true"/>；否则为 <see langword="false"/>。 如果在原始 <see langword="true"/> 中没有找到 key，则此方法也会返回 <see cref="IObject"/>。</returns>
		public bool Remove (ISection item)
		{
			if (item == null)
			{
				throw new ArgumentNullException ("item");
			}

			int index = this._sortedList.IndexOfValue (item);
			if (index != -1)
			{
				this._sortedList.RemoveAt (index);
				return true;
			}
			return false;
		}
		/// <summary>
		/// 返回循环访问 <see cref="IObject"/> 的枚举数
		/// </summary>
		/// <returns><see cref="IEnumerator"/> 的类型 <see cref="IEnumerator{ISection}"/> 的 <see cref="IObject"/></returns>
		public IEnumerator<ISection> GetEnumerator ()
		{
			return this._sortedList.Values.GetEnumerator ();
		}
		/// <summary>
		/// 返回一个循环访问集合的枚举器
		/// </summary>
		/// <returns>用于循环访问集合的枚举数</returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}
		/// <summary>
		/// 如果元素数小于当前容量的 90%，将容量设置为 <see cref="IObject"/> 中的实际元素数。
		/// </summary>
		public void TrimExcess ()
		{
			this._sortedList.TrimExcess ();
		}
		/// <summary>
		/// 获取与指定键关联的值
		/// </summary>
		/// <param name="key">要获取其值的键</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。 此参数未经初始化即被传递</param>
		/// <exception cref="ArgumentNullException">key 为 null</exception>
		/// <returns>如果 <see langword="true"/> 包含具有指定键的元素，则为 <see cref="IObject"/>；否则为 <see langword="false"/></returns>
		public bool TryGetValue (string key, out ISection value)
		{
			return this._sortedList.TryGetValue (key, out value);
		}
		/// <summary>
		/// 确定此实例是否与另一个指定的 <see cref="IObject"/> 对象具有相同的值
		/// </summary>
		/// <param name="other">要与实例进行比较的 <see cref="IObject"/></param>
		/// <returns>如果 <see langword="true"/> 参数的值与此实例的值相同，则为 value；否则为 <see langword="false"/>。 如果 value 为 null，则此方法返回 <see langword="false"/></returns>
		public bool Equals (IObject other)
		{
			return other != null && IObject.Equals (this, other);
		}
		/// <summary>
		/// 确定此实例是否与指定的对象（也必须是 <see cref="IObject"/> 对象）具有相同的值
		/// </summary>
		/// <param name="obj">要与实例进行比较的 <see cref="IObject"/></param>
		/// <returns>如果 <see langword="true"/> 参数的值与此实例的值相同，则为 value；否则为 <see langword="false"/>。 如果 value 为 null，则此方法返回 <see langword="false"/></returns>
		public override bool Equals (object obj)
		{
			return this.Equals (obj as IObject);
		}
		/// <summary>
		/// 返回该实例的哈希代码
		/// </summary>
		/// <returns>32 位有符号整数哈希代码</returns>
		public override int GetHashCode ()
		{
			return this._fileName.GetHashCode () & this._sortedList.GetHashCode ();
		}
		/// <summary>
		/// 返回表示当前对象的字符串
		/// </summary>
		/// <returns>表示当前对象的字符串</returns>
		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder ();
			foreach (ISection item in this._sortedList.Values)
			{
				builder.AppendLine (item.ToString ());
			}
			return builder.ToString ();
		}
		#endregion
		#region 私有方法
		/// <summary>
		/// 处理 <see cref="Uri"/> 实例, 将其变为可直接使用的对象
		/// </summary>
		/// <param name="fileUri">文件路径的 <see cref="Uri"/> 对象</param>
		/// <returns>返回处理过的 Uri</returns>
		private static Uri ConvertAbsoluteUri(Uri fileUri)
		{
			if (!fileUri.IsAbsoluteUri)
			{
				// 处理原始字符串
				StringBuilder urlBuilder = new StringBuilder(fileUri.OriginalString);
				urlBuilder.Replace("/", "\\");
				while (urlBuilder[0] == '\\')
				{
					urlBuilder.Remove(0, 1);
				}

				// 将相对路径转为绝对路径
				urlBuilder.Insert(0, AppDomain.CurrentDomain.BaseDirectory);
				fileUri = new Uri(urlBuilder.ToString(), UriKind.Absolute);
			}

			return fileUri;
		}
		/// <summary>
		/// 逐行解析 Ini 配置文件字符串
		/// </summary>
		/// <param name="textReader"></param>
		/// <returns></returns>
		private static IObject ParseIni(TextReader textReader, string fileName)
		{
			IObject iniObj = new IObject(fileName);
			ISection iniSect = null;
			while (textReader.Peek() != -1)
			{
				string line = textReader.ReadLine();
				if (string.IsNullOrEmpty(line) == false && Regices[2].IsMatch(line) == false)     //跳过空行和注释
				{
					Match match = Regices[0].Match(line);
					if (match.Success)
					{
						iniSect = new ISection(match.Groups[1].Value);
						iniObj.Add(iniSect);
						continue;
					}

					match = Regices[1].Match(line);
					if (match.Success)
					{
						iniSect.Add(match.Groups[1].Value.Trim(), match.Groups[2].Value);
					}
				}
			}
			return iniObj;
		}
		#endregion
	}
}
