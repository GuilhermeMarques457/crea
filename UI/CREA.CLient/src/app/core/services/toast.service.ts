import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly snackBar = inject(MatSnackBar);

  success(message: string) {
    this.snackBar.open(message, 'Fechar', { duration: 3000, panelClass: 'success-snack' });
  }

  error(message: string) {
    this.snackBar.open(message, 'Fechar', { duration: 5000, panelClass: 'error-snack' });
  }
}
