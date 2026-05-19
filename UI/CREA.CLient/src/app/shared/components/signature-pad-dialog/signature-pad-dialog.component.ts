import {
  Component,
  inject,
  AfterViewInit,
  ViewChild,
  ElementRef,
  OnDestroy,
  signal,
} from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import SignaturePad from 'signature_pad';

@Component({
  selector: 'app-signature-pad-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './signature-pad-dialog.component.html',
  styles: `
    .signature-canvas {
      display: block;
      width: 100%;
      border: 2px dashed #cbd5e1;
      border-radius: 12px;
      cursor: crosshair;
      touch-action: none;
      background: #fff;
      box-sizing: border-box;
    }
    .signature-canvas.active {
      border-color: #3b82f6;
    }
  `,
})
export class SignaturePadDialogComponent implements AfterViewInit, OnDestroy {
  private dialogRef = inject(MatDialogRef<SignaturePadDialogComponent>);
  private signaturePad!: SignaturePad;
  private resizeObserver!: ResizeObserver;

  @ViewChild('canvas', { static: true }) canvasRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('canvasWrapper', { static: true }) wrapperRef!: ElementRef<HTMLDivElement>;

  isEmpty = signal(true);

  ngAfterViewInit() {
    const canvas = this.canvasRef.nativeElement;
    this.signaturePad = new SignaturePad(canvas, {
      penColor: '#1e293b',
      backgroundColor: '#ffffff',
    });

    this.signaturePad.addEventListener('beginStroke', () => {
      this.isEmpty.set(false);
    });

    this.resizeCanvas();
    this.resizeObserver = new ResizeObserver(() => this.resizeCanvas());
    this.resizeObserver.observe(this.wrapperRef.nativeElement);
  }

  ngOnDestroy() {
    this.resizeObserver?.disconnect();
  }

  private resizeCanvas() {
    const canvas = this.canvasRef.nativeElement;
    const ratio = window.devicePixelRatio || 1;
    const width = this.wrapperRef.nativeElement.clientWidth;
    const height = 200;
    canvas.width = width * ratio;
    canvas.height = height * ratio;
    canvas.style.width = `${width}px`;
    canvas.style.height = `${height}px`;
    canvas.getContext('2d')!.scale(ratio, ratio);
    this.signaturePad?.clear();
    this.isEmpty.set(true);
  }

  limpar() {
    this.signaturePad.clear();
    this.isEmpty.set(true);
  }

  confirmar() {
    if (this.signaturePad.isEmpty()) return;
    const dataUrl = this.signaturePad.toDataURL('image/png');
    this.dialogRef.close(dataUrl);
  }

  cancelar() {
    this.dialogRef.close(null);
  }
}
