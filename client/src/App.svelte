<script lang="ts">
  import {
    Header,
    Content,
    SkipToContent,
    HeaderUtilities,
    Button
  } from 'carbon-components-svelte';
  import MaplibreMap from './lib/Map.svelte';
  import Earthquake from "carbon-icons-svelte/lib/Earthquake.svelte";
  import 'carbon-components-svelte/css/g100.css';

  let earthquake_rendered: boolean = true;
  let animation_trigger: boolean = false;
</script>

<main>
  <Header company="ГИИБ">
    <svelte:fragment slot="skip-to-content">
      <SkipToContent />
    </svelte:fragment>

    <HeaderUtilities>
      <Button kind="ghost" icon={Earthquake} iconDescription="On/Off Animation" tooltipPosition="left" on:click={()=> {animation_trigger = !animation_trigger}}>
      </Button>
    </HeaderUtilities>
  </Header>
  
  <Content>
    <MaplibreMap  bind:animation_trigger={animation_trigger}/>
  </Content>
</main>

<style>
</style>
