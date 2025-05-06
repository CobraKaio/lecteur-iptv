<template>
  <div :class="positionClasses">
    <Notification
      v-for="notification in notifications"
      :key="notification.id"
      :message="notification.message"
      :type="notification.type"
      :position="position"
      :timeout="notification.timeout"
      :auto-close="notification.autoClose"
      @close="removeNotification(notification.id)"
    />
  </div>
</template>

<script setup>
import { computed } from 'vue';
import Notification from './Notification.vue';

const props = defineProps({
  notifications: {
    type: Array,
    default: () => []
  },
  position: {
    type: String,
    default: 'top-right',
    validator: (value) => ['top-right', 'top-left', 'bottom-right', 'bottom-left', 'top-center', 'bottom-center'].includes(value)
  }
});

const emit = defineEmits(['remove']);

// Classes de position
const positionClasses = computed(() => {
  switch (props.position) {
    case 'top-right':
      return 'fixed top-0 right-0 z-50 p-4 space-y-4';
    case 'top-left':
      return 'fixed top-0 left-0 z-50 p-4 space-y-4';
    case 'bottom-right':
      return 'fixed bottom-0 right-0 z-50 p-4 space-y-4';
    case 'bottom-left':
      return 'fixed bottom-0 left-0 z-50 p-4 space-y-4';
    case 'top-center':
      return 'fixed top-0 left-1/2 transform -translate-x-1/2 z-50 p-4 space-y-4';
    case 'bottom-center':
      return 'fixed bottom-0 left-1/2 transform -translate-x-1/2 z-50 p-4 space-y-4';
    default:
      return 'fixed top-0 right-0 z-50 p-4 space-y-4';
  }
});

// Supprimer une notification
const removeNotification = (id) => {
  emit('remove', id);
};
</script>
