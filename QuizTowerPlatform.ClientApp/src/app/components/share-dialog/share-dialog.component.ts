import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import {
  MatDialog,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogTitle,
} from '@angular/material/dialog';

@Component({
  selector: 'app-share-dialog',
  templateUrl: './share-dialog.component.html',
})
export class ShareDialogComponent implements OnInit {
  platformName: string = '';
  shareUrl: string = '';
  message: string = '';

  constructor(@Inject(MAT_DIALOG_DATA) public data: { platform: string; url: string, message: string }) {
    this.platformName = data.platform;
    this.shareUrl = data.url;
    this.message = data.message;
  }

  ngOnInit(): void {
    if(this.message.length > 0)
    {
      return;
    }
    this.openSharePage();
  }

  openSharePage(): void {
    const width = 528;
    const height = 454;
    const screenWidth = window.screen.width;
    const screenHeight = window.screen.height;
    const left = (screenWidth - width) / 2;
    const top = (screenHeight - height) / 2;
    window.open(this.data.url,'_blank',`toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=${width},height=${height},top=${top},left=${left}`);
  }
}
