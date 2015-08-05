using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// ���[�h�����v���O�C���̃��^�f�[�^�ƁA�C���|�[�g���ꂽ�v���O�C���@�\�ւ̃A�N�Z�X��񋟂��܂��B
	/// </summary>
	public class Plugin : IEnumerable
	{
		private readonly List<object> functions = new List<object>();

		public Guid Id { get; }

		public PluginMetadata Metadata { get; }

		internal Plugin(IPluginMetadata metadata)
		{
			this.Id = new Guid(metadata.Guid);
			this.Metadata = new PluginMetadata
			{
				Title = metadata.Title,
				Description = metadata.Description,
				Version = metadata.Version,
				Author = metadata.Author,
			};
		}

		internal void Add<TContract>(TContract function) where TContract : class
		{
			this.functions.Add(function);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.functions.GetEnumerator();
		}
	}
}
