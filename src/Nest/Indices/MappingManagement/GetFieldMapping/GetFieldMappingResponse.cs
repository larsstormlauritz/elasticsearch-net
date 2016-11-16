﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace Nest
{
	public class FieldMappingProperties : Dictionary<string, FieldMapping> { }

	public class TypeFieldMappings
	{
		[JsonProperty("mappings")]
		public IReadOnlyDictionary<string, FieldMappingProperties> Mappings { get; internal set; } = EmptyReadOnly<string, FieldMappingProperties>.Dictionary;
	}

	public class FieldMapping
	{
		[JsonProperty("full_name")]
		public string FullName { get; internal set; }

		[JsonProperty("mapping")]
		[JsonConverter(typeof(FieldMappingJsonConverter))]
		public IReadOnlyDictionary<string, IFieldMapping> Mapping { get; internal set; } = EmptyReadOnly<string, IFieldMapping>.Dictionary;
	}

	public interface IGetFieldMappingResponse : IResponse
	{
		IReadOnlyDictionary<string, TypeFieldMappings> Indices { get; }

		IFieldMapping MappingFor(string indexName, string typeName, string fieldName);

		IFieldMapping MappingFor<T>(string fieldName)
			where T : class;

		IFieldMapping MappingFor<T>(Expression<Func<T, object>> fieldName)
			where T : class;

		FieldMappingProperties MappingsFor<T>(string indexName = null, string typeName = null)
			where T : class;

		FieldMappingProperties MappingsFor(string indexName, string typeName);
	}

	public class GetFieldMappingResponse : ResponseBase, IGetFieldMappingResponse
	{
		private Inferrer _inferrer { get; set; }

		public GetFieldMappingResponse() { }

		internal GetFieldMappingResponse(IApiCallDetails status, IReadOnlyDictionary<string, TypeFieldMappings> dict, Inferrer inferrer)
		{
			this.Indices = dict ?? EmptyReadOnly<string, TypeFieldMappings>.Dictionary;
			this._inferrer = inferrer;
			//TODO can dict truely ever be null, whats the response look like when field mapping is not found.
			//does status.Success not already reflect this?
			//this.IsValid = status.Success && dict != null && dict.Count > 0;
		}

		public IReadOnlyDictionary<string, TypeFieldMappings> Indices { get; internal set; } = EmptyReadOnly<string, TypeFieldMappings>.Dictionary;

		public FieldMappingProperties MappingsFor(string indexName, string typeName)
		{
			TypeFieldMappings index;
			FieldMappingProperties type;

			if (!this.Indices.TryGetValue(indexName, out index) || index.Mappings == null) return null;
			if (!index.Mappings.TryGetValue(typeName, out type)) return null;

			return type;
		}

		public IFieldMapping MappingFor(string indexName, string typeName, string fieldName)
		{
			if (fieldName.IsNullOrEmpty()) return null;

			var type = this.MappingsFor(indexName, typeName);
			if (type == null) return null;

			FieldMapping field;
			if (!type.TryGetValue(fieldName, out field) || field.Mapping == null) return null;

			var name = fieldName.Split('.').Last();
			return field.Mapping[name];
		}

		public IFieldMapping MappingFor<T>(string fieldName)
			where T : class
		{
			var indexName = this._inferrer.IndexName<T>();
			var typeName = this._inferrer.TypeName<T>();
			return this.MappingFor(indexName, typeName, fieldName);
		}

		public IFieldMapping MappingFor<T>(Expression<Func<T, object>> fieldName)
			where T : class
		{
			var path = this._inferrer.Field(fieldName);
			return this.MappingFor<T>(path);
		}

		public FieldMappingProperties MappingsFor<T>(string indexName = null, string typeName = null)
			where T : class
		{
			indexName = indexName ?? this._inferrer.IndexName<T>();
			typeName = typeName ?? this._inferrer.TypeName<T>();

			return this.MappingsFor(indexName, typeName);
		}
	}
}
