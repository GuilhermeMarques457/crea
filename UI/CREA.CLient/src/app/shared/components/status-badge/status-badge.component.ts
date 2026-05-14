import { Component, input } from '@angular/core';
import { StatusObra, STATUS_OBRA_LABELS } from '../../models/api.models';

const STATUS_CLASSES: Record<StatusObra, string> = {
  [StatusObra.EmAndamento]: 'bg-blue-100 text-blue-800',
  [StatusObra.Pausada]: 'bg-yellow-100 text-yellow-800',
  [StatusObra.Concluida]: 'bg-green-100 text-green-800',
  [StatusObra.Cancelada]: 'bg-red-100 text-red-800',
};

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `
    <span
      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium {{
        cssClass()
      }}"
    >
      {{ label() }}
    </span>
  `,
})
export class StatusBadgeComponent {
  status = input.required<StatusObra>();
  label = () => STATUS_OBRA_LABELS[this.status()];
  cssClass = () => STATUS_CLASSES[this.status()];
}
