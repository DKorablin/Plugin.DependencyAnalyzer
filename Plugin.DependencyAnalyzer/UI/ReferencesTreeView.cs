using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AlphaOmega.Debug;
using AlphaOmega.Debug.CorDirectory.Meta;
using AlphaOmega.Debug.CorDirectory.Meta.Tables;
using AlphaOmega.Debug.NTDirectory;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer.UI
{
	internal class ReferencesTreeView : TreeView
	{
		private const String EXPORT_TABLE_NAME = "Export Table";
		private static readonly Dictionary<NodeState, Color> State = new Dictionary<NodeState, Color>()
		{
			{ NodeState.Used, Color.Empty },
			{ NodeState.NotUsed, Color.Gray },
			{ NodeState.Missed, Color.Red },
		};

		private enum TreeImageType
		{
			Folder = 0,
			Assembly = 1,
			Namespace = 2,
			Class = 3,
			Method = 4,
			Property = 5,
		}

		public enum NodeState
		{
			Used,
			NotUsed,
			Missed,
		}

		public sealed class ShowResult
		{
			public Int32 Used { get; set; }
			public Int32 NotUsed { get; set; }
			public Int32 Missed { get; set; }

			public void Increment(Color nodeColor)
			{
				foreach(NodeState state in Enum.GetValues(typeof(NodeState)))
					if(nodeColor == State[state])
						this[state]++;
			}

			public Int32 this[NodeState state]
			{
				get
				{
					switch(state)
					{
					case NodeState.Missed:
						return this.Missed;
					case NodeState.NotUsed:
						return this.NotUsed;
					case NodeState.Used:
						return this.Used;
					default:
						throw new NotSupportedException();
					}
				}
				set
				{
					switch(state)
					{
					case NodeState.Missed:
						this.Missed = value;
						break;
					case NodeState.NotUsed:
						this.NotUsed = value;
						break;
					case NodeState.Used:
						this.Used = value;
						break;
					default:
						throw new NotImplementedException();
					}
				}
			}
		}

		private sealed class ReferenceType
		{
			public TypeRefRow[] TypeRef { get; set; }
			public MemberRefRow[] MemberRef { get; set; }
		}

		private sealed class ReferenceModule
		{
			public String ModuleRef { get; set; }
			public ImplMapRow[] ImplMap { get; set; }
		}

		private sealed class DefinedType
		{
			public DefinedType ParentClass { get; set; }
			public TypeDefRow TypeDef { get; private set; }
			public MethodDefRow[] MethodDef { get; private set; }
			public FieldRow[] Fields { get; private set; }

			public IEnumerable<DefinedType> ReverseHierarchy()
			{
				List<DefinedType> reverse = new List<DefinedType>
				{
					this
				};

				DefinedType parent = this.ParentClass;
				while(parent != null)
				{
					reverse.Insert(0, parent);
					parent = parent.ParentClass;
				}
				return reverse.ToArray();
			}

			public DefinedType(TypeDefRow row)
			{
				this.TypeDef = row;
				List<MethodDefRow> methods = new List<MethodDefRow>();
				foreach(MethodDefRow method in row.MethodList)
					if(((method.Flags & MethodAttributes.Public) == MethodAttributes.Public)
						|| (method.Flags & MethodAttributes.Family) == MethodAttributes.Family)//Protected
						methods.Add(method);
				this.MethodDef = methods.ToArray();

				List<FieldRow> fields = new List<FieldRow>();
				foreach(FieldRow field in row.FieldList)
					if(((field.Flags & FieldAttributes.Public) == FieldAttributes.Public)
						|| (field.Flags & FieldAttributes.Family) == FieldAttributes.Family)//Protected
						fields.Add(field);
				this.Fields = fields.ToArray();
			}
		}

		private Boolean _highlightReferencedMembers;
		private Library _parentLibrary;

		public Boolean HighlightReferencedMembers
		{
			get => this._highlightReferencedMembers;
			set
			{
				this._highlightReferencedMembers = value;

				if(this._highlightReferencedMembers)
					this.ShowDefinedMembers(this.ParentLibrary);
			}
		}

		public Library ParentLibrary
		{
			get => this._parentLibrary;
			set
			{
				this._parentLibrary = value;
				if(this.HighlightReferencedMembers)
					this.ShowDefinedMembers(value);
			}
		}

		private void ShowDefinedMembers(Library lib)
		{
			this.Nodes.Clear();
			if(lib == null) return;

			using(PEFile file = lib.OpenPE())
			{
				if(file.ComDescriptor != null && file.ComDescriptor.MetaData != null)
				{
					StreamTables tables = file.ComDescriptor.MetaData.StreamTables;
					foreach(DefinedType type in ReferencesTreeView.GetDefinedTypes(tables).OrderBy(t => t.TypeDef.TypeNamespace).ThenBy(t => t.TypeDef.TypeName))
					{
						TreeNode nodeNamespace = null;
						TreeNode nodeType = null;
						foreach(DefinedType nestedType in type.ReverseHierarchy())
						{
							if(nodeNamespace == null)
							{
								nodeNamespace = GetOrAddNode(this.Nodes, nestedType.TypeDef.TypeNamespace, TreeImageType.Namespace);
								nodeType = GetOrAddNode(nodeNamespace.Nodes, nestedType.TypeDef.TypeName, TreeImageType.Class);
							} else
								nodeType = GetOrAddNode(nodeType.Nodes, nestedType.TypeDef.TypeName, TreeImageType.Class);
						}

						foreach(MethodDefRow method in type.MethodDef)
							GetOrAddNode(nodeType.Nodes, method.Name, TreeImageType.Method);

						foreach(FieldRow field in type.Fields)
							GetOrAddNode(nodeType.Nodes, field.Name, TreeImageType.Property);
					}
				}

				if(file.Export?.IsEmpty == false)
				{
					TreeNode exportTable = GetOrAddNode(this.Nodes, ReferencesTreeView.EXPORT_TABLE_NAME, TreeImageType.Folder);
					foreach(ExportFunction function in file.Export.GetExportFunctions())
						GetOrAddNode(exportTable.Nodes, function.Name, TreeImageType.Method);
				}
			}
		}

		public ShowResult ShowReferencedMembers(IEnumerable<Library> childAssemblies)
		{
			ShowResult result = new ShowResult();
			this.SuspendLayout();
			try
			{
				this.ClearReferences(this.Nodes);
				foreach(Library lib in childAssemblies)
					this.ShowReferencedMembers(lib);

				foreach(TreeNode node in EnumerateNodes(this.Nodes))
				{
					switch((TreeImageType)node.ImageIndex)
					{
					case TreeImageType.Folder:
					case TreeImageType.Namespace:
						//node.Expand();
						break;
					case TreeImageType.Class:
					case TreeImageType.Method:
					case TreeImageType.Property:
						result.Increment(node.ForeColor);
						break;
					}
				}

				/*if(this.Nodes.Count > 0)
					this.Nodes[0].EnsureVisible();*/
			} finally
			{
				this.ResumeLayout();
			}

			return result;
		}

		private void ShowReferencedMembers(Library childAssembly)
		{
			using(PEFile file = childAssembly.OpenPE())
			{
				if(file.ComDescriptor?.IsEmpty == false)
				{
					StreamTables tables = file.ComDescriptor.MetaData.StreamTables;
					foreach(ReferenceType type in this.GetReferencedTypes(tables))
					{
						TreeNode nodeNamespace = this.GetNode(this.Nodes, type.TypeRef[0].TypeNamespace, TreeImageType.Namespace);
						TreeNode nodeType = nodeNamespace;
						for(Int32 loop = 0; loop < type.TypeRef.Length; loop++)
							nodeType = this.GetNode(nodeType.Nodes, type.TypeRef[loop].TypeName, TreeImageType.Class);

						foreach(MemberRefRow member in type.MemberRef)
							this.GetNode(nodeType.Nodes, member.Name, TreeImageType.Method);
					}

					ReferenceModule refModule = this.GetReferenceModule(tables);
					if(refModule != null)
					{
						TreeNode exportTable = GetOrAddNode(this.Nodes, ReferencesTreeView.EXPORT_TABLE_NAME, TreeImageType.Folder);
						foreach(ImplMapRow map in refModule.ImplMap)
						{
							TreeNode found = FindNode(exportTable.Nodes, (node) => { return node.Text == map.ImportName; }).FirstOrDefault();
							if(found == null && !map.ImportName.EndsWith("W"))//Trying to find Unicode alternative (MethodNameW)
								found = FindNode(exportTable.Nodes, (node) => { return node.Text == map.ImportName + "W"; }).FirstOrDefault();
							if(found == null && !map.ImportName.EndsWith("A"))//Trying to find Asci alternative (MethodNameA)
								found = FindNode(exportTable.Nodes, (node) => { return node.Text == map.ImportName + "A"; }).FirstOrDefault();

							String importName = found == null ? map.ImportName : found.Text;
							this.GetNode(exportTable.Nodes, importName, TreeImageType.Method);
						}
					}
				}

				String parentAssemblyFileName = Path.GetFileName(this.ParentLibrary.Path).ToLowerInvariant();
				foreach(ImportModule importModule in file.Import)
					if(String.Equals(parentAssemblyFileName, importModule.ModuleName.ToLowerInvariant()))
					{
						TreeNode nodeExportTable = this.GetNode(this.Nodes, ReferencesTreeView.EXPORT_TABLE_NAME, TreeImageType.Folder);
						foreach(WinNT.IMAGE_IMPORT_BY_NAME module in importModule)
						{
							if(module.IsByOrdinal)
							{
								Boolean isFound = false;
								foreach(TreeNode node in nodeExportTable.Nodes)
								{
									ExportFunction func = (ExportFunction)node.Tag;
									if(func.Ordinal == module.Hint)
									{
										isFound = true;
										HighlightNode(nodeExportTable.Nodes, null, node, TreeImageType.Method);
										break;
									}
								}
								if(!isFound)
									this.GetNode(nodeExportTable.Nodes, "@" + module.Hint, TreeImageType.Method);
							} else
								this.GetNode(nodeExportTable.Nodes, module.Name, TreeImageType.Method);
						}
					}
			}
		}

		private static IEnumerable<DefinedType> GetDefinedTypes(StreamTables tables)
		{
			BaseMetaTable<TypeDefRow> typeDefTable = tables.TypeDef;

			foreach(TypeDefRow type in typeDefTable)
				switch(type.VisibilityMask)
				{
				case TypeAttributes.Public:
					yield return new DefinedType(type);
					break;
				case TypeAttributes.NestedPublic:
					yield return new DefinedType(type) { ParentClass = GetParentClassRec(tables, type), };
					break;
				}
		}

		private static DefinedType GetParentClassRec(StreamTables tables, TypeDefRow typeDef)
		{
			BaseMetaTable<NestedClassRow> nestedClassTable = tables.NestedClass;
			
			foreach(NestedClassRow row in nestedClassTable)
				if(row.NestedClass == typeDef)
				{
					DefinedType result = new DefinedType(row.EnclosingClass);
					if(row.EnclosingClass.VisibilityMask == TypeAttributes.NestedPublic)
						result.ParentClass = GetParentClassRec(tables, row.EnclosingClass);
					return result;
				}
			return null;
		}

		private IEnumerable<ReferenceType> GetReferencedTypes(StreamTables tables)
		{
			BaseMetaTable<TypeRefRow> typeRefTable = tables.TypeRef;
			BaseMetaTable<MemberRefRow> memberRefTable = tables.MemberRef;

			foreach(TypeRefRow type in typeRefTable)
			{
				TypeRefRow[] typeRef = GetOwnedRowRec(tables, type);

				AssemblyRefRow assembly = tables.AssemblyRef[typeRef[0].ResolutionScope.RowIndex.Value];
				if(this.ParentLibrary.Equals(assembly.Name, assembly.Version))
				{
					List<MemberRefRow> members = new List<MemberRefRow>();
					foreach(MemberRefRow member in memberRefTable)
					{
						switch(member.Class.TableType)
						{
						case Cor.MetaTableType.TypeRef:
							var typeCheck = typeRefTable[member.Class.RowIndex.Value];
							if(typeCheck.Index == type.Index)//Checking that current member is owned by type
								members.Add(member);
							break;
						}
					}
					yield return new ReferenceType() { TypeRef = typeRef, MemberRef = members.ToArray(), };
				}
			}
		}

		private static TypeRefRow[] GetOwnedRowRec(StreamTables tables, TypeRefRow child)
		{
			List<TypeRefRow> result = new List<TypeRefRow>
			{
				child
			};

			switch(child.ResolutionScope.TableType)
			{
			case Cor.MetaTableType.TypeRef:
				TypeRefRow parent = child;
				while(parent.ResolutionScope.TableType == Cor.MetaTableType.TypeRef)
				{
					parent = tables.TypeRef[parent.ResolutionScope.RowIndex.Value];
					result.Insert(0, parent);
				}
				break;
			case Cor.MetaTableType.ModuleRef://Reference to native module
				//ModuleRefRow mParent = tables.ModuleRef[child.ResolutionScope.RowIndex.Value];
				break;
			case Cor.MetaTableType.AssemblyRef:
				break;
			default:
				throw new NotImplementedException();
			}

			return result.ToArray();
		}

		private ReferenceModule GetReferenceModule(StreamTables tables)
		{
			BaseMetaTable<ModuleRefRow> moduleRefTable = tables.ModuleRef;
			BaseMetaTable<ImplMapRow> implMapTable = tables.ImplMap;

			//.NET will not optimize ModuleName, that's why in ModuleRef can be many instances of one library with different case. Example: kernel32.dll, Kernel32.dll, KerNel32.dll,...
			String libraryName = this.ParentLibrary.Name.ToLowerInvariant();
			List<ImplMapRow> members = new List<ImplMapRow>();
			foreach(ModuleRefRow module in moduleRefTable)
				if(String.Equals(libraryName, module.Name.ToLowerInvariant()))
				{
					foreach(ImplMapRow member in implMapTable)
						if(member.ImportScope.Index == module.Index)
							members.Add(member);
				}
			return members.Count > 0
				? new ReferenceModule() { ModuleRef = this.ParentLibrary.Name, ImplMap = members.ToArray(), }
				: null;
		}

		private void ClearReferences(TreeNodeCollection nodes)
		{
			if(this.HighlightReferencedMembers)
			{
				for(Int32 loop = nodes.Count - 1; loop >= 0; loop--)
				{
					if(nodes[loop].ForeColor == State[NodeState.Missed])
						nodes[loop].Remove();
					else
					{
						nodes[loop].ForeColor = State[NodeState.NotUsed];
						this.ClearReferences(nodes[loop].Nodes);
					}
				}
			}
			else
				nodes.Clear();
		}

		private TreeNode GetNode(TreeNodeCollection nodes, String text, TreeImageType image)
			=> this.HighlightReferencedMembers
				? HighlightNode(nodes, text, image)
				: GetOrAddNode(nodes, text, image);

		private static TreeNode GetOrAddNode(TreeNodeCollection nodes, String text, TreeImageType image)
		{
			foreach(TreeNode node in FindNode(nodes, node => node.Text == text))
				return node;

			TreeNode result = new TreeNode(text) { ImageIndex = (Int32)image, SelectedImageIndex = (Int32)image, };
			nodes.Add(result);
			return result;
		}

		private static TreeNode HighlightNode(TreeNodeCollection nodes, String text, TreeImageType image)
		{
			foreach(TreeNode node in FindNode(nodes, node => node.Text == text))
				return HighlightNode(nodes, text, node, image);

			return HighlightNode(nodes, text, null, image);
		}

		private static TreeNode HighlightNode(TreeNodeCollection nodes, String text, TreeNode node, TreeImageType image)
		{
			if(node == null)
			{
				TreeNode result = new TreeNode(text) { ForeColor = State[NodeState.Missed], ImageIndex = (Int32)image, SelectedImageIndex = (Int32)image, };
				nodes.Add(result);
				return result;
			} else if(node.ForeColor == State[NodeState.NotUsed])
				node.ForeColor = State[NodeState.Used];

			return node;
		}

		private static IEnumerable<TreeNode> FindNode(TreeNodeCollection nodes, Func<TreeNode, Boolean> func)
		{
			foreach(TreeNode node in nodes)
				if(func(node))
					yield return node;
		}

		private static IEnumerable<TreeNode> EnumerateNodes(TreeNodeCollection nodes)
		{
			foreach(TreeNode node in nodes)
			{
				yield return node;
				foreach(TreeNode subNode in EnumerateNodes(node.Nodes))
					yield return subNode;
			}
		}
	}
}
