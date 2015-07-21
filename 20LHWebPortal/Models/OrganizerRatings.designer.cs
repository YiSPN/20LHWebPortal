﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _20LHWebPortal.Models
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="aspnet-20LHWebPortal-20150420113045")]
	public partial class OrganizerRatingsDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertOrganizerRating(OrganizerRating instance);
    partial void UpdateOrganizerRating(OrganizerRating instance);
    partial void DeleteOrganizerRating(OrganizerRating instance);
    #endregion
		
		public OrganizerRatingsDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["aspnet_20LHWebPortal_20150420113045ConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public OrganizerRatingsDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OrganizerRatingsDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OrganizerRatingsDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OrganizerRatingsDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<OrganizerRating> OrganizerRatings
		{
			get
			{
				return this.GetTable<OrganizerRating>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.OrganizerRatings")]
	public partial class OrganizerRating : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _OrganizerId;
		
		private string _AttendeeId;
		
		private System.Nullable<int> _Rating;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnOrganizerIdChanging(string value);
    partial void OnOrganizerIdChanged();
    partial void OnAttendeeIdChanging(string value);
    partial void OnAttendeeIdChanged();
    partial void OnRatingChanging(System.Nullable<int> value);
    partial void OnRatingChanged();
    #endregion
		
		public OrganizerRating()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OrganizerId", DbType="NVarChar(128) NOT NULL", CanBeNull=false)]
		public string OrganizerId
		{
			get
			{
				return this._OrganizerId;
			}
			set
			{
				if ((this._OrganizerId != value))
				{
					this.OnOrganizerIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizerId = value;
					this.SendPropertyChanged("OrganizerId");
					this.OnOrganizerIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AttendeeId", DbType="NVarChar(128) NOT NULL", CanBeNull=false)]
		public string AttendeeId
		{
			get
			{
				return this._AttendeeId;
			}
			set
			{
				if ((this._AttendeeId != value))
				{
					this.OnAttendeeIdChanging(value);
					this.SendPropertyChanging();
					this._AttendeeId = value;
					this.SendPropertyChanged("AttendeeId");
					this.OnAttendeeIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Rating", DbType="Int")]
		public System.Nullable<int> Rating
		{
			get
			{
				return this._Rating;
			}
			set
			{
				if ((this._Rating != value))
				{
					this.OnRatingChanging(value);
					this.SendPropertyChanging();
					this._Rating = value;
					this.SendPropertyChanged("Rating");
					this.OnRatingChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
