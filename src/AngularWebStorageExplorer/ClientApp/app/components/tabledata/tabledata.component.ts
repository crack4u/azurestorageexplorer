﻿import { Component, Inject, Input, ViewChild } from '@angular/core';
import { UtilsService } from '../../services/utils/utils.service';
import { BaseComponent } from '../base/base.component';

@Component({
	selector: 'tabledata',
	templateUrl: './tabledata.component.html'
})

export class TabledataComponent extends BaseComponent {
	forceReload: boolean;

	@Input() storageTable: string = "";
	@ViewChild('inputQuery') inputQuery: any;

	public data: any;
	public loading: boolean = false;
	public showTable: boolean = false;

	public headers: string[] = [];
	public rows: object[] = [];

	constructor(utils: UtilsService) {
		super(utils);
		this.getData();
	}

	ngOnChanges() {
		this.getData();
	}

	getData() {
		if (!this.storageTable)
			return;

		this.showTable = false;
		this.data = null;
		this.loading = true;
		this.utilsService.getData('api/Tables/QueryTable?table=' + this.storageTable + '&query=').subscribe(result => {
			this.data = result.json();
			this.processData();
		}, error => { this.setErrorMessage(error.statusText); });
	}

	processData() {
		//https://stackoverflow.com/questions/1232040/how-do-i-empty-an-array-in-javascript
		this.headers.length = 0;
		this.rows.length = 0;

		this.headers.push("Partition Key");
		this.headers.push("Row Key");
		for (let entry of this.data) {
			let values: string[] = entry.values.split(";");
			var obj: any = {};
			obj["Partition Key"] = entry.partitionKey;
			obj["Row Key"] = entry.rowKey;
			for (let v of values) {
				if (!v) continue;
				let pair: string[] = v.split("=");
				if (!this.headers.find(h => h === pair[0]))
					this.headers.push(pair[0]);
				obj[pair[0]] = pair[1];
			}
			this.rows.push(obj);
		}
		this.loading = false;
		this.showTable = true;
	}
}