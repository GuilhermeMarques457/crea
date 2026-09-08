import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AssinaturaService } from '../../../core/services/assinatura.service';
import { ToastService } from '../../../core/services/toast.service';
import { PreviewEmailAssinaturaDto, TipoEntidadeAssinatura } from '../../models/api.models';
import { TIPO_ASSINANTE_LABELS } from '../../utils/assinatura.utils';

export interface EnviarEmailAssinaturaDialogData {
  tipoEntidade: TipoEntidadeAssinatura;
  entidadeId: string;
  linkAssinatura: string;
}

@Component({
  selector: 'app-enviar-email-assinatura-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './enviar-email-assinatura-dialog.component.html',
})
export class EnviarEmailAssinaturaDialogComponent implements OnInit {
  private readonly data = inject<EnviarEmailAssinaturaDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<EnviarEmailAssinaturaDialogComponent>);
  private readonly assinaturaService = inject(AssinaturaService);
  private readonly toast = inject(ToastService);

  readonly labels = TIPO_ASSINANTE_LABELS;

  loading = signal(true);
  sending = signal(false);
  erro = signal<string | null>(null);
  preview = signal<PreviewEmailAssinaturaDto | null>(null);

  ngOnInit(): void {
    this.assinaturaService.destinatariosEmail(this.data.tipoEntidade, this.data.entidadeId).subscribe({
      next: (preview) => {
        this.preview.set(preview);
        if (!preview.destinatarios.length) {
          this.erro.set('Não há destinatário pendente de assinatura com e-mail cadastrado.');
        }
        this.loading.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os destinatários.');
        this.loading.set(false);
      },
    });
  }

  emailsResumo(): string {
    return (this.preview()?.destinatarios ?? []).map((d) => d.email).join(', ');
  }

  confirmar(): void {
    const preview = this.preview();
    if (!preview?.destinatarios.length || this.sending()) return;

    this.sending.set(true);
    this.assinaturaService
      .enviarEmail({
        tipoEntidade: this.data.tipoEntidade,
        entidadeId: this.data.entidadeId,
        linkAssinatura: this.data.linkAssinatura,
      })
      .subscribe({
        next: (res) => {
          this.toast.success(res.mensagem);
          this.dialogRef.close(true);
        },
        error: (err) => {
          const mensagem =
            err?.error?.mensagem ?? 'Não foi possível enviar o e-mail. Tente novamente.';
          this.toast.error(mensagem);
          this.sending.set(false);
        },
      });
  }
}
