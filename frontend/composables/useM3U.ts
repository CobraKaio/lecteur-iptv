import { ref, computed } from 'vue'

export interface Channel {
  id: string;
  name: string;
  logo?: string;
  group?: string;
  url: string;
}

export function useM3U() {
  const channels = ref<Channel[]>([]);
  const isLoading = ref(false);
  const error = ref<string | null>(null);
  
  const groupedChannels = computed(() => {
    const groups: Record<string, Channel[]> = {};
    
    channels.value.forEach(channel => {
      const group = channel.group || 'Non classé';
      if (!groups[group]) {
        groups[group] = [];
      }
      groups[group].push(channel);
    });
    
    return groups;
  });
  
  async function loadFromUrl(url: string) {
    isLoading.value = true;
    error.value = null;
    
    try {
      const response = await fetch(url);
      if (!response.ok) {
        throw new Error(`Erreur HTTP: ${response.status}`);
      }
      
      const content = await response.text();
      parseM3U(content);
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur inconnue';
      channels.value = [];
    } finally {
      isLoading.value = false;
    }
  }
  
  function parseM3U(content: string) {
    const lines = content.split('\n');
    if (!lines[0].includes('#EXTM3U')) {
      error.value = 'Format M3U invalide';
      channels.value = [];
      return;
    }
    
    const parsedChannels: Channel[] = [];
    let currentChannel: Partial<Channel> | null = null;
    
    for (let i = 1; i < lines.length; i++) {
      const line = lines[i].trim();
      
      if (line.startsWith('#EXTINF:')) {
        // Nouvelle chaîne
        currentChannel = { id: crypto.randomUUID() };
        
        // Extraction du nom et des attributs
        const infoMatch = line.match(/#EXTINF:-1\s+(.*),(.*)$/);
        if (infoMatch && infoMatch.length >= 3) {
          const attributes = infoMatch[1];
          const name = infoMatch[2].trim();
          
          currentChannel.name = name;
          
          // Extraction du logo
          const logoMatch = attributes.match(/tvg-logo="([^"]*)"/);
          if (logoMatch) {
            currentChannel.logo = logoMatch[1];
          }
          
          // Extraction du groupe
          const groupMatch = attributes.match(/group-title="([^"]*)"/);
          if (groupMatch) {
            currentChannel.group = groupMatch[1];
          }
        }
      } else if (line.length > 0 && !line.startsWith('#') && currentChannel) {
        // URL de la chaîne
        currentChannel.url = line;
        
        if (currentChannel.name && currentChannel.url) {
          parsedChannels.push(currentChannel as Channel);
        }
        
        currentChannel = null;
      }
    }
    
    channels.value = parsedChannels;
  }
  
  return {
    channels,
    groupedChannels,
    isLoading,
    error,
    loadFromUrl,
    parseM3U
  };
}
